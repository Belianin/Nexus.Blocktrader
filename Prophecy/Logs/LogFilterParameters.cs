
using Nexus.Logging.Utils;

namespace Nexus.Prophecy.Logs
{
    public class LogFilterParameters
    {
        public string? Substring { get; set; }
        public bool IsIgnoreCase { get; set; } = true;
        public LogLevel[]? LogLevels { get; set; }
    }
}