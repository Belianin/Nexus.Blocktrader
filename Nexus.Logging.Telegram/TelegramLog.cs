using System;
using System.Collections.Generic;
using Nexus.Logging.Utils;
using Telegram.Bot;

namespace Nexus.Logging.Telegram
{
    public class TelegramLog : BaseLog
    {
        private readonly HashSet<long> channels;
        private readonly ITelegramBotClient client;
        private readonly ILog outerLog;

        public TelegramLog(string token, ILog outerLog = null, params long[] channels)
        {
            this.outerLog = outerLog;
            this.channels = new HashSet<long>(channels);
            try
            {

                client = new TelegramBotClient(token);
                client.StartReceiving();
            }
            catch (Exception e)
            {
                outerLog?.Fatal($"TelegramLog failed to start: {e.Message}");
            }
        }
        
        protected override void InnerLog(LogEvent logEvent)
        {
            try
            {
                foreach (var channel in channels)
                {
                    client.SendTextMessageAsync(channel, FormatLogEvent(logEvent))
                        .GetAwaiter().GetResult();
                }
            }
            catch (Exception e)
            {
                outerLog?.Fatal($"TelegramLog is dead: {e.Message}");
            }
        }

        public override void Dispose()
        {
           client.StopReceiving();
        }

        private string FormatLogEvent(LogEvent logEvent)
        {
            return $"{FormatLogLevel(logEvent.Level)} " +
                   $"{(logEvent.Context != null ? $"[{logEvent.Context}] " : string.Empty)}" +
                   $"{logEvent.Message}"; 
        }
        
        private string FormatLogLevel(LogLevel level) => level switch
        {
            LogLevel.Debug => $"🛠#debug",
            LogLevel.Info => $"💬#info",
            LogLevel.Warn => $"⚠#warn",
            LogLevel.Error => $"⛔#error",
            LogLevel.Fatal => $"💀#fatal",
            LogLevel.Important => $"‼️#important",
            _ => $"#wtf"
        };
    }
}