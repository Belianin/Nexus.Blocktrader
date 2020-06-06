using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Nexus.Blocktrader.Service;
using Nexus.Blocktrader.Service.Files;
using Nexus.Logging;

namespace Blocktrader.Worker
{
    public class FetchingWorker : BackgroundService
    {
        private readonly ILog log;
        private bool isUpdating;
        private readonly TimeSpan updateInterval = TimeSpan.FromMinutes(10);
        private readonly BlocktraderService service;
        private readonly ITimestampManager timestampManager;

        public FetchingWorker(ILog log)
        {
            this.log = log.ForContext("FetchingWorker");
            
            service = new BlocktraderService(this.log);
            timestampManager = new FileTimestampManager(this.log);

            this.log.Info("Fetching worker initializated");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var sleepTime = (int) GetSleepInterval();
                log.Debug($"Next download scheduled at {DateTime.Now.AddMilliseconds(sleepTime):yyyy-MM-dd HH:mm:ss,fff}");
                await Task.Delay(sleepTime, stoppingToken);
                
                await DownloadAsync().ConfigureAwait(false);
            }
        }

        private double GetSleepInterval()
        {
            return updateInterval.TotalMilliseconds - DateTime.Now.TimeOfDay.TotalMilliseconds % updateInterval.TotalMilliseconds;
        }
        
        private async Task DownloadAsync()
        {
            isUpdating = true;
            var timestamp = await service.GetCurrentTimestampAsync().ConfigureAwait(false);
            await timestampManager.WriteAsync(timestamp);
            isUpdating = false;
        }
    }
}