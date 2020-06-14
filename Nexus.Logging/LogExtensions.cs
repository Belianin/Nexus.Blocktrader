using System;
using Nexus.Logging.Utils;

namespace Nexus.Logging
{
    public static class LogExtensions
    {
        public static void Debug(this ILog log, string message)
            => log.Log(FormLogEvent(LogLevel.Debug, message));
        public static void Info(this ILog log, string message)
            => log.Log(FormLogEvent(LogLevel.Info, message));
        public static void Warn(this ILog log, string message)
            => log.Log(FormLogEvent(LogLevel.Warn, message));
        public static void Error(this ILog log, string message)
            => log.Log(FormLogEvent(LogLevel.Error, message));
        public static void Error(this ILog log, Exception exception)
            => log.Log(FormLogEvent(LogLevel.Error, exception.Message));
        public static void Fatal(this ILog log, string message)
            => log.Log(FormLogEvent(LogLevel.Fatal, message));
        public static void Important(this ILog log, string message)
            => log.Log(FormLogEvent(LogLevel.Important, message));

        public static ILog ForContext(this ILog log, string context) => new ContextLog(log, context);

        public static ILog OnlyErrors(this ILog log)
        {
            log.SetEnabled(LogLevel.Debug, false);
            log.SetEnabled(LogLevel.Info, false);
            log.SetEnabled(LogLevel.Warn, false);
            log.SetEnabled(LogLevel.Error, true);
            log.SetEnabled(LogLevel.Fatal, true);
            log.SetEnabled(LogLevel.Important, true);

            return log;
        }
        
        private static LogEvent FormLogEvent(LogLevel logLevel, string message)
        {
            return new LogEvent(logLevel, message);
        }
    }
}