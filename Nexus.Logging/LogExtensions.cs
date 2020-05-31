using System;
using Nexus.Logging.Utils;

namespace Nexus.Logging
{
    public static class LogExtensions
    {
        public static void Debug(this ILog log, string message, params object[] parameters)
            => log.Log(FormLogEvent(LogLevel.Debug, message, parameters));
        public static void Info(this ILog log, string message, params object[] parameters)
            => log.Log(FormLogEvent(LogLevel.Info, message, parameters));
        public static void Warn(this ILog log, string message, params object[] parameters)
            => log.Log(FormLogEvent(LogLevel.Warn, message, parameters));
        public static void Error(this ILog log, string message, params object[] parameters)
            => log.Log(FormLogEvent(LogLevel.Error, message, parameters));
        public static void Error(this ILog log, Exception exception)
            => log.Log(FormLogEvent(LogLevel.Error, exception.Message));
        public static void Fatal(this ILog log, string message, params object[] parameters)
            => log.Log(FormLogEvent(LogLevel.Fatal, message, parameters));
        public static void Important(this ILog log, string message, params object[] parameters)
            => log.Log(FormLogEvent(LogLevel.Important, message, parameters));

        public static ILog ForContext(this ILog log, string context) => new ContextLog(log, context);
        
        private static LogEvent FormLogEvent(LogLevel logLevel, string message, params object[] parameters)
        {
            return new LogEvent(logLevel, message, null, parameters);
        }
    }
}