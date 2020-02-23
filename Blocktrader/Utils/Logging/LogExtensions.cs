namespace Blocktrader.Utils.Logging
{
    public static class LogExtensions
    {
        public static void Debug(this ILog log, string message) => log.Log(message, LogLevel.Debug);
        public static void Info(this ILog log, string message) => log.Log(message, LogLevel.Info);
        public static void Warn(this ILog log, string message) => log.Log(message, LogLevel.Warning);
        public static void Error(this ILog log, string message) => log.Log(message, LogLevel.Error);
        public static void Fatal(this ILog log, string message) => log.Log(message, LogLevel.Fatal);
    }
}