using System;

namespace Blocktrader.Utils.Logging
{
    public class ConsoleLog : ILog
    {
        public void Log(string message, LogLevel logLevel)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss,fff} [{logLevel.ToString()}] {message}");
        }
    }
}