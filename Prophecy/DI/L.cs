using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nexus.Logging;
using Nexus.Logging.Console;
using Nexus.Logging.File;
using Nexus.Prophecy.Notifications;
using Nexus.Prophecy.Notifications.Telegram;

namespace Nexus.Prophecy.DI
{
    public static class NotificatorDIExtensions
    {
        public static IServiceCollection AddNotificatorService(this IServiceCollection services)
        {
            services.AddSingleton<INotificationService>(sp =>
            {
                var log = sp.GetRequiredService<ILog>();
                var telegramSettings = GetTelegramSettings();
                var notificator = new TelegramNotificator(telegramSettings.Token, log, telegramSettings.LogChannels);

                return new NotificationService(new [] {notificator});
            });

            return services;
        }

        private static TelegramSettings GetTelegramSettings()
        {
            var text = File.ReadAllText("telegram.settings.json");

            return JsonConvert.DeserializeObject<TelegramSettings>(text);
        }

        private class TelegramSettings
        {
            public string Token { get; set; }
            public long[] LogChannels { get; set; }
        }
    }
}