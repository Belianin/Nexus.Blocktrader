using System;
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
                    .UseStartup<Startup>()
                    .UseUrls("http://*:777")
                    .ConfigureLogging(l => l.AddConsole().SetMinimumLevel(LogLevel.Debug)))
                .ConfigureServices(services => services.AddHostedService<FetchingWorker>().AddLogging(l => l.AddConsole().SetMinimumLevel(LogLevel.Debug)));
    }
}