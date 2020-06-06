using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Service;
using Nexus.Blocktrader.Service.Trades;
using Nexus.Logging;

namespace Blocktrader.Worker
{
    public class TradesFetchingWorker : BackgroundService
    {
        private readonly ILog log;
        private readonly AlignedScheduler scheduler;
        private readonly Dictionary<ExchangeTitle, int> lastIds = new Dictionary<ExchangeTitle, int>
        {
            [ExchangeTitle.Binance] = 0,
            [ExchangeTitle.Bitfinex] = 0,
            [ExchangeTitle.Bitstamp] = 0
        };

        private readonly int minimumAmount = 10;

        public TradesFetchingWorker(ILog log)
        {
            this.log = log.ForContext("TradesFetcher");
            
            var service = new BlocktraderService(this.log);
            var manager = new FileTradesManager(this.log);
            
            async Task DownloadTradeListsAsync()
            {
                var tradeLists = await service.GetCurrentTradeListsAsync().ConfigureAwait(false);

                foreach (var (exchange, trades) in tradeLists)
                {
                    var filtered = FilterTrades(exchange, trades);

                    await manager.WriteAsync(exchange, filtered, Ticker.BtcUsd).ConfigureAwait(false);
                }
            }
            
            scheduler = new AlignedScheduler(DownloadTradeListsAsync, TimeSpan.FromSeconds(30), this.log);

            log.Info("TradesFetcher initializated");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await scheduler.RunAsync(stoppingToken).ConfigureAwait(false);
        }

        private Trade[] FilterTrades(ExchangeTitle exchange, Trade[] trades)
        {
            var log = this.log.ForContext(exchange.ToString());
            
            var newTrades = trades.Where(t => t.Id > lastIds[exchange]).ToArray();
            log.Debug($"Got {newTrades.Length}/{trades.Length} new trades (with id bigger than {lastIds[exchange]})");
            
            var result = newTrades.Where(t => t.Amount > minimumAmount).ToArray();
            log.Debug($"Got {result.Length}/{newTrades.Length} with amount bigger than {minimumAmount}");

            if (newTrades.Length > 0)
            {
                var maxId = newTrades.Max(t => t.Id);
                log.Debug($"New last id is {maxId}");
                lastIds[exchange] = maxId;
            }

            return result;
        }
    }
}