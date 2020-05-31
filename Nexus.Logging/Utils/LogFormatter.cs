using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Nexus.Logging.Utils
{
    public static class LogFormatter
    {
        public static string Format(LogEvent logEvent)
        {
            return $"{logEvent.DateTime.ToString("yyyy-MM-dd HH:mm:ss,fff", CultureInfo.CurrentCulture)}" +
                   $"{TagToString(logEvent.Level.ToString())}" +
                   $"{logEvent.Message}";
        }
        
        private static string TagToString(string tag)
        {
            return TagsToString(new[] {tag});
        }
        
        private static string TagsToString(ICollection<string> tags)
        {
            if (tags == null || tags.Count == 0)
                return " ";

            var sb = new StringBuilder();
            sb.Append(" ");
            foreach (var tag in tags)
                sb.Append($"[{tag}] ");

            return sb.ToString();
        }
    }
}