using System;

namespace Blocktrader.Utils.Logging
{
    public class ColorConsoleLog: ILog
    {
        public void Log(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss,fff} [{logLevel.ToString()}] {message}");
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss,fff} [{logLevel.ToString()}] {message}");
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss,fff} [{logLevel.ToString()}] {message}");
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss,fff} [{logLevel.ToString()}] {message}");
                    break;
                case LogLevel.Fatal:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss,fff} [{logLevel.ToString()}] {message}");
                    break;
            }
        }
    }
}