using System.Collections.Generic;

namespace Nexus.Logging.Utils
{
    public abstract class BaseLog : ILog
    {
        private readonly HashSet<LogLevel> disabledLevels = new HashSet<LogLevel>();

        public void SetEnabled(LogLevel level, bool isEnabled)
        {
            if (isEnabled)
                disabledLevels.Remove(level);
            else
                disabledLevels.Add(level);
        }

        public void Log(LogEvent logEvent)
        {
            if (!disabledLevels.Contains(logEvent.Level))
                InnerLog(logEvent);
        }

        protected abstract void InnerLog(LogEvent logEvent);

        public abstract void Dispose();
    }
}