using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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

        private static string TagsToString(LogEvent logEvent)
        {
            var tags = logEvent.Context != null
                ? new[] {logEvent.Level.ToString(), logEvent.Context}
                : new[] {logEvent.Level.ToString()};

            return string.Join(" ", tags.Select(t => $"[{t}]"));
        }
    }
}