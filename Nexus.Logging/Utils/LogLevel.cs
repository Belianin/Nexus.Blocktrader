using System.ComponentModel;

namespace Nexus.Logging.Utils
{
    public enum LogLevel
    {
        [Description("DEBUG")]
        Debug,
        [Description("INFO")]
        Info,
        [Description("WARN")]
        Warn,
        [Description("ERROR")]
        Error,
        [Description("FATAL")]
        Fatal,
        [Description("IMPORTANT")]
        Important
    }
}