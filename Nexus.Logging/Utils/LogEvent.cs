using System;

namespace Nexus.Logging.Utils
{
    public class LogEvent
    {
        public LogLevel Level { get; }
        public DateTime DateTime { get; }
        public string Message { get; }
        public string Context { get; set; }
        public object[] Parameters { get; }

        public LogEvent(LogLevel level, string message, string context = null, params object[] parameters)
        {
            Level = level;
            Message = message;
            Context = context;
            DateTime = DateTime.Now;
            Parameters = parameters;
        }
    }
}