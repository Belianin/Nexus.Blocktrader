using System;
using System.Collections.Generic;
using Nexus.Logging.Utils;

namespace Nexus.Logging
{
    public class AggregationLog : BaseLog, IDisposable
    {
        private readonly IList<ILog> logs;

        public AggregationLog(params ILog[] logs)
        {
            this.logs = new List<ILog>(logs);
        }
        
        protected override void InnerLog(LogEvent logEvent)
        {
            foreach (var log in logs) 
                log.Log(logEvent);
        }

        public override void Dispose()
        {
            foreach (var log in logs) 
                log.Dispose();
        }
    }
}