using System;
using System.Collections.Generic;
using Blocktrader.Worker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nexus.Blocktrader.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .ConfigureLogging(config => { config.ClearProviders(); })
                    .UseStartup<Startup>()
                    .UseUrls("http://*:777"))
                .ConfigureServices(services =>
                    services.AddHostedService<FetchingWorker>())
                .ConfigureLogging(config => { config.ClearProviders(); })
                .ConfigureServices(services =>
                    services.AddHostedService<TradesFetchingWorker>())
                .ConfigureLogging(config => { config.ClearProviders(); });
    }
}