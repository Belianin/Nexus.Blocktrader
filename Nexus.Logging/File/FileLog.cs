using System;
using System.Globalization;
using System.IO;
using Nexus.Logging.Utils;

namespace Nexus.Logging.File
{
    public class FileLog : BaseLog, IDisposable
    {
        private readonly StreamWriter writer;
        
        public FileLog(string fileName = null)
        {
            fileName ??= GetFileName(DateTime.Now);

            if (!System.IO.File.Exists(fileName))
                System.IO.File.Create(fileName);
            
            writer = new StreamWriter(new BufferedStream(System.IO.File.OpenWrite(fileName)));
        }

        protected override void InnerLog(LogEvent logEvent)
        {
            writer.WriteLine(LogFormatter.Format(logEvent));
        }

        public override void Dispose()
        {
            writer.Dispose();
        }
        
        public static string GetFileName(DateTime dateTime) =>
            $"log_{dateTime.ToString("yyyy-MM", CultureInfo.InvariantCulture)}.txt";
    }
}