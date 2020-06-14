using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Logging.Utils;

namespace Nexus.Logging.File
{
    public class FileLog : BaseLog
    {
        private readonly ConcurrentQueue<LogEvent> eventsQueue = new ConcurrentQueue<LogEvent>();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        
        public FileLog()
        {
            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");
            
            Task.Run(() => WriteLogsAsync(cts.Token), cts.Token);
        }

        protected override void InnerLog(LogEvent logEvent)
        {
            eventsQueue.Enqueue(logEvent);
        }

        public override void Dispose()
        {
            cts.Cancel();
        }
        
        public static string GetFileName(DateTime dateTime) =>
            $"Logs/log_{dateTime.ToString("yyyy-MM", CultureInfo.InvariantCulture)}.txt";

        private async Task WriteLogsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (eventsQueue.IsEmpty)
                    await Task.Delay(1000, token).ConfigureAwait(false);
                else
                {
                    var fileName = GetFileName(DateTime.Now);

                    if (!System.IO.File.Exists(fileName))
                        System.IO.File.Create(fileName);
                    
                    await using var writer = new StreamWriter(fileName, true);
                    {
                        while (eventsQueue.TryDequeue(out var logEvent))
                        {
                            await writer.WriteLineAsync(LogFormatter.Format(logEvent));
                        }
                    }
                }
            }
        }
    }
}