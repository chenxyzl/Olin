using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;

namespace Gate.Model
{
    public class ClusterClientService : IHostedService
    {
        private readonly ILogger<ClusterClientService> _logger;
        private readonly IConfiguration _config;

        public ClusterClientService(ILogger<ClusterClientService> logger, ILoggerProvider loggerProvider,
            IConfiguration config)
        {
            _logger = logger;
            _config = config;

            Client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = _config.GetSection("ClusterConfig")["ClusterId"];
                    options.ServiceId = _config.GetSection("ClusterConfig")["ServiceId"];
                })
                .Configure<SchedulingOptions>(options =>
                {
                    options.AllowCallChainReentrancy = true;
                    options.PerformDeadlockDetection = true;
                })
                .Configure<HostOptions>(options => { options.ShutdownTimeout = TimeSpan.FromMinutes(1); })
                //.ConfigureServices()
                .ConfigureLogging(builder => builder.AddProvider(loggerProvider))
                .ConfigureApplicationParts(parts => { parts.AddFromApplicationBaseDirectory().WithReferences(); })
                //.UseLocalhostClustering()
                .UseMongoDBClient(config.GetSection("persistenceOptions")["connectionString"])
                .UseMongoDBClustering(options =>
                {
                    options.DatabaseName = config.GetSection("persistenceOptions")["databaseName"];
                })
                .Build();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var attempt = 0;
            var maxAttempts = int.MaxValue;
            var delay = TimeSpan.FromSeconds(1);
            return Client.Connect(async error =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                if (++attempt < maxAttempts)
                {
                    _logger.LogWarning(error,
                        "Failed to connect to Orleans cluster on attempt {@Attempt} of {@MaxAttempts}.",
                        attempt, maxAttempts);

                    try
                    {
                        await Task.Delay(delay, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    _logger.LogError(error,
                        "Failed to connect to Orleans cluster on attempt {@Attempt} of {@MaxAttempts}.",
                        attempt, maxAttempts);

                    return false;
                }
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Client.Close();
            }
            catch (OrleansException error)
            {
                _logger.LogWarning(error,
                    "Error while gracefully disconnecting from Orleans cluster. Will ignore and continue to shutdown.");
            }
        }

        public IClusterClient Client { get; }
    }
}