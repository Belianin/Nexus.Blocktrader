using System;
using System.Collections.Generic;

namespace Nexus.Logging.Utils
{
    public class LogEvent
    {
        public LogLevel Level { get; }
        public DateTime DateTime { get; }
        public string Message { get; }
        public Stack<string> Context { get; set; }

        public LogEvent(LogLevel level, string message, params string[] context)
        {
            Level = level;
            Message = message;
            Context = new Stack<string>(context);
            DateTime = DateTime.Now;
        }
    }
}