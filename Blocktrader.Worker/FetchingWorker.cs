using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.Blocktrader.Service;
using Nexus.Blocktrader.Service.Files;
using Nexus.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

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
            this.log = log;
            
            service = new BlocktraderService(log);
            timestampManager = new FileTimestampManager(log);

            log.Info("Fetcher worker initializated");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DownloadAsync().ConfigureAwait(false);
                await Task.Delay(updateInterval, stoppingToken);
            }
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