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
                    .ConfigureLogging(config => {
                        config.ClearProviders();
                    })
                    .UseStartup<Startup>()
                    .UseUrls("http://*:777"))
                .ConfigureServices(services => 
                    services.AddHostedService<FetchingWorker>())
                .ConfigureLogging(config => {
                    config.ClearProviders();
                });
    }
    
    public class NexusLogger : ILogger
    {
        private readonly HashSet<LogLevel> disabledLogLevels = new HashSet<LogLevel>();
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return !disabledLogLevels.Contains(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}