using System.Globalization;
using System.Linq;

namespace Nexus.Logging.Utils
{
    internal static class LogFormatter
    {
        public static string Format(LogEvent logEvent)
        {
            return $"{logEvent.DateTime.ToString("yyyy-MM-dd HH:mm:ss,fff", CultureInfo.CurrentCulture)} " +
                   $"{TagsToString(logEvent)} " +
                   $"{logEvent.Message}";
        }

        private static string TagsToString(LogEvent logEvent)
        {
            var tags = logEvent.Context != null
                ? new[] {logEvent.Level.ToString().ToUpper()}.Concat(logEvent.Context)
                : new[] {logEvent.Level.ToString().ToUpper()};

            return string.Join(" ", tags.Select(t => $"[{t}]"));
        }
    }
}