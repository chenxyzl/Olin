﻿using Gate.Model;
using Gate.Model.Network;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MO.Gateway.Network;
using NLog.Extensions.Logging;

namespace Gate.Boot
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ClusterClientService>();
                    services.AddSingleton<IHostedService>(_ => _.GetService<ClusterClientService>()!);
                    services.AddSingleton(_ => _.GetService<ClusterClientService>()!.Client);

                    services.AddHostedService<WebSocketService>();
                    services.AddHostedService<SocketService>();
                    services.Configure<ConsoleLifetimeOptions>(options => { options.SuppressStatusMessages = true; });
                })
                .ConfigureAppConfiguration(builder =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("config.json", false, false);
                })
                .ConfigureLogging(builder =>
                {
                    //builder.AddConsole();
                    builder.ClearProviders();
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddNLog();
                });
        }
    }
}