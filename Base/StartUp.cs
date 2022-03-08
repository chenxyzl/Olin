using Orleans.Configuration;
using Orleans.Hosting;
using Base.OrleansExtensions;

namespace Base;

public class StartUp
{
    class A : IStopTask
    {
        public Task Execute(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public ISiloHost Build()
    {
        int a = 1;
        int b = a.TT();
        Console.WriteLine(b);
        var silo = new SiloHostBuilder()
            // Clustering information
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "my-first-cluster";
                options.ServiceId = "MyAwesomeOrleansService";
            })
            .AddStartupTask((IServiceProvider services, CancellationToken cancellation) => Task.CompletedTask)
            .AddStopTask((IServiceProvider services, CancellationToken cancellation) => Task.CompletedTask)
            // Clustering provider
            // .UseAzureStorageClustering(options => options.ConnectionString = connectionString)
            // Endpoints
            .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
            // Application parts: just reference one of the grain implementations that we use
            // .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ValueGrain).Assembly).WithReferences())
            // Now create the silo!
            .Build();
        return silo;
    }
}