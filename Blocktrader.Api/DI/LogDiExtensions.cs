using System.IO;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Logging;
using Nexus.Logging.Console;
using Nexus.Logging.File;
using Nexus.Logging.Prophecy;
using Nexus.Logging.Telegram;

namespace Nexus.Blocktrader.Api.DI
{
    public static class LogDiExtensions
    {
        public static IServiceCollection AddLogs(this IServiceCollection services)
        {
            services.AddSingleton<ILog>(sp =>
            {
                var url = "http://localhost:5080";
                
                var regularLog = new AggregationLog(new FileLog(), new ColourConsoleLog());
                var telegramLog = new ProphecyLog(url, regularLog)
                    .OnlyErrors();
                
                return new AggregationLog(new AggregationLog(regularLog, telegramLog));
            });

            return services;
        }
    }
}