using System;
using System.Globalization;
using System.Linq;

namespace Nexus.Logging.Utils
{
    public static class LogFormatter
    {
        public static string Format(LogEvent logEvent)
        {
            return $"{logEvent.DateTime.ToString("yyyy-MM-dd HH:mm:ss,fff", CultureInfo.CurrentCulture)} " +
                   $"{TagsToString(logEvent)} " +
                   $"{logEvent.Message}";
        }

        public static bool IsLogLevel(string log, LogLevel level) => log.Contains($"[{level.ToString().ToUpper()}]");

        public static DateTime GetLogTime(string log) => DateTime.ParseExact(
            log.Substring(0, 23), "yyyy-MM-dd HH:mm:ss,fff", CultureInfo.InvariantCulture);

        private static string TagsToString(LogEvent logEvent)
        {
            var tags = logEvent.Context != null
                ? new[] {logEvent.Level.ToString().ToUpper()}.Concat(logEvent.Context)
                : new[] {logEvent.Level.ToString().ToUpper()};

            return string.Join(" ", tags.Select(t => $"[{t}]"));
        }
    }
}