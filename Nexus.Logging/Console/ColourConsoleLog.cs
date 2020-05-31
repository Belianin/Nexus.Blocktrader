using System;
using Nexus.Logging.Utils;

namespace Nexus.Logging.Console
{
    public class ColourConsoleLog : BaseLog
    {
        protected override void InnerLog(LogEvent logEvent)
        {
            System.Console.ForegroundColor = GetColor(logEvent.Level);
            System.Console.WriteLine(LogFormatter.Format(logEvent));
        }

        private static ConsoleColor GetColor(LogLevel level) => level switch
        {
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Warn => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Fatal => ConsoleColor.DarkRed,
            LogLevel.Important => ConsoleColor.Green,
            _ => ConsoleColor.Magenta
        };

        public override void Dispose() {}
    }
}