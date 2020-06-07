using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Nexus.Blocktrader.Service;
using Nexus.Blocktrader.Service.Timestamps;
using Nexus.Logging;

namespace Blocktrader.Worker
{
    public class FetchingWorker : BackgroundService
    {
        private readonly AlignedScheduler scheduler;

        public FetchingWorker(ILog log)
        {
            log = log.ForContext("FetchingWorker");
            
            var service = new BlocktraderService(log);
            var manager = new FileTimestampManager(log);
            
            async Task DownloadTimestampsAsync()
            {
                var timestamp = await service.GetCurrentTimestampAsync().ConfigureAwait(false);
                await manager.WriteAsync(timestamp);
            }
            
            scheduler = new AlignedScheduler(DownloadTimestampsAsync, TimeSpan.FromMinutes(10), log);

            log.Info("Fetching worker initializated");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await scheduler.RunAsync(stoppingToken).ConfigureAwait(false);
        }
    }
}