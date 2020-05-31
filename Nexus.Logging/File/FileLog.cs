using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Logging.Utils;

namespace Nexus.Logging.File
{
    public class FileLog : BaseLog
    {
        private readonly string fileName;
        private readonly ConcurrentQueue<LogEvent> eventsQueue = new ConcurrentQueue<LogEvent>();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        
        public FileLog(string fileName = null)
        {
            this.fileName = fileName ?? GetFileName(DateTime.Now);

            if (!System.IO.File.Exists(this.fileName))
                System.IO.File.Create(this.fileName);

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
            $"log_{dateTime.ToString("yyyy-MM", CultureInfo.InvariantCulture)}.txt";

        private async Task WriteLogsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (eventsQueue.IsEmpty)
                    await Task.Delay(1000, token).ConfigureAwait(false);
                else
                {
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