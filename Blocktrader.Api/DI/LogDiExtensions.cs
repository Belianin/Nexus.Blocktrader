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
            return services.AddSingleton<ILog>(sp => new AggregationLog(new FileLog(), new ColourConsoleLog()));
        }
    }
}