using System;
using Nexus.Logging.Utils;

namespace Nexus.Logging
{
    public interface ILog : IDisposable
    {
        public void Log(LogEvent logEvent);
        public void SetEnabled(LogLevel level, bool isEnabled);
    }
}