using System.IO;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Logging;
using Nexus.Logging.Console;
using Nexus.Logging.File;
using Nexus.Logging.Telegram;

namespace Nexus.Blocktrader.Api.DI
{
    public static class LogDiExtensions
    {
        public static IServiceCollection AddLogs(this IServiceCollection services)
        {
            services.AddSingleton<ILog>(sp =>
            {
                var telegramSettings = GetTelegramSettings();
                
                var regularLog = new AggregationLog(new FileLog(), new ColourConsoleLog());
                var telegramLog = new TelegramLog(telegramSettings.Token, regularLog, telegramSettings.LogChannels)
                    .OnlyErrors();
                
                return new AggregationLog(new AggregationLog(regularLog, telegramLog));
            });

            return services;
        }

        private static TelegramSettings GetTelegramSettings()
        {
            var text = File.ReadAllText("telegram.settings.json");

            return JsonSerializer.Deserialize<TelegramSettings>(text);
        }

        private class TelegramSettings
        {
            public string Token { get; set; }
            public long[] LogChannels { get; set; }
        }
    }
}