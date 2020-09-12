using System;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Logging;

namespace Nexus.Blocktrader.Worker
{
    public class FlexScheduler
    {
        private readonly ILog log;
        private readonly Func<Task<TimeSpan>> action;

        public FlexScheduler(Func<Task<TimeSpan>> action, ILog log)
        {
            this.action = action;
            this.log = log;
        }

        public async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var sleepTime = await action().ConfigureAwait(false);
                log.Debug($"Sleep time: {sleepTime:g}. Next action scheduled at {DateTime.Now.Add(sleepTime):yyyy-MM-dd HH:mm:ss,fff}");
                await Task.Delay(sleepTime, token);
            }
        }
    }
}