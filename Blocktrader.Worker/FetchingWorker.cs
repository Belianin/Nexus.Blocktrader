using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Nexus.Blocktrader.Timestamps;
using Nexus.Logging;

namespace Nexus.Blocktrader.Worker
{
    public class FetchingWorker : BackgroundService
    {
        private readonly AlignedScheduler scheduler;

        public FetchingWorker(ILog log)
        {
            var proxySettings = File.Exists("proxy.settings.json")
                ? JsonConvert.DeserializeObject<ExchangeProxySettings>(File.ReadAllText("proxy.settings.json"))
                : new ExchangeProxySettings();
            log = log.ForContext("FetchingWorker");
            
            var service = new BlocktraderService(log, proxySettings);
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