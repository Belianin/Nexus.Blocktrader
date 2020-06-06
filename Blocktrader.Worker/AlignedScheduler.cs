using System;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Logging;

namespace Blocktrader.Worker
{
    public class AlignedScheduler
    {
        private readonly ILog log;
        private readonly Func<Task> action;
        private readonly TimeSpan interval;

        public AlignedScheduler(Func<Task> action, TimeSpan interval, ILog log)
        {
            this.log = log;
            this.action = action;
            this.interval = interval;
        }

        public async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var sleepTime = (int) GetSleepInterval();
                log.Debug($"Next action scheduled at {DateTime.Now.AddMilliseconds(sleepTime):yyyy-MM-dd HH:mm:ss,fff}");
                await Task.Delay(sleepTime, token);
                
                await action().ConfigureAwait(false);
            }
        }

        private double GetSleepInterval()
        {
            return interval.TotalMilliseconds - DateTime.Now.TimeOfDay.TotalMilliseconds % interval.TotalMilliseconds;
        }
    }
}