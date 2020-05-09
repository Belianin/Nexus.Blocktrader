using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.Blocktrader.Service;
using Nexus.Blocktrader.Service.Files;
using Nexus.Blocktrader.Utils.Logging;

namespace Blocktrader.Worker
{
    public class FetchingWorker : BackgroundService
    {
        private readonly ILogger<FetchingWorker> logger;
        private bool isUpdating;
        private readonly TimeSpan updateInterval = TimeSpan.FromMinutes(10);
        private readonly BlocktraderService service;
        private readonly ITimestampManager timestampManager;

        public FetchingWorker(ILogger<FetchingWorker> logger)
        {
            this.logger = logger;
            
            var log = new ConsoleLog(); // ILogger
            service = new BlocktraderService(new ConsoleLog());
            timestampManager = new FileTimestampManager(log);

            log.Info($"Blocktader initializated");
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