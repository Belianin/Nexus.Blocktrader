using Nexus.Logging.Utils;

namespace Nexus.Logging
{
    public class ContextLog : ILog
    {
        private readonly ILog innerLog;
        private readonly string context;

        internal ContextLog(ILog log, string context)
        {
            innerLog = log;
            this.context = context;
        }

        public void Log(LogEvent logEvent)
        {
            logEvent.Context.Push(context);
            innerLog.Log(logEvent);
        }

        public void SetEnabled(LogLevel level, bool isEnabled)
        {
            innerLog.SetEnabled(level, isEnabled);
        }

        public void Dispose()
        {
            innerLog.Dispose();
        }
    }
}