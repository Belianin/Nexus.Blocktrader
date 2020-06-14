
using Nexus.Logging.Utils;

namespace Nexus.Prophecy.Notifications
{
    public class Notification
    {
        public LogLevel LogLevel { get; }

        public string Message { get; }
        
        public string[] Context { get; set; }

        public Notification(LogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;
        }
    }
}