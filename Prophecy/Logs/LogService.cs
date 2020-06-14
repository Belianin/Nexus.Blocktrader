using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nexus.Core;
using Nexus.Logging.File;
using Nexus.Logging.Utils;

namespace Nexus.Prophecy.Logs
{
    public class LogService : ILogService
    {
        private readonly IDictionary<string, string> registeredServices;

        public LogService(IDictionary<string, string> registeredServices)
        {
            this.registeredServices = registeredServices;
        }

        public Result<IEnumerable<string>> GetLogs(string service, DateTime @from, DateTime to)
        {
            if (!registeredServices.ContainsKey(service))
                return $"Service \"{service}\" not found";

            var filename = registeredServices[service.ToLower()] + FileLog.GetFileName(from);
            if (!File.Exists(filename))
                return $"No logs for this period";

            return Result.Ok(File
                .ReadLines(filename)
                .SkipWhile(s => LogFormatter.GetLogTime(s) < from)
                .TakeWhile(s => LogFormatter.GetLogTime(s) <= to));
        }
    }
}