using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nexus.Blocktrader.Models;
using Nexus.Blocktrader.Trades;
using Nexus.Logging;

namespace Nexus.Blocktrader.Worker
{
    public class TradesFetchingWorker : StatefulWorker<TradesFetcherState>
    {
        private readonly ILog log;
        private readonly AlignedScheduler scheduler;

        public TradesFetchingWorker(ILog log)
        {
            var proxySettings = File.Exists("proxy.settings.json")
                ? JsonConvert.DeserializeObject<ExchangeProxySettings>(File.ReadAllText("proxy.settings.json"))
                : new ExchangeProxySettings();
            this.log = log.ForContext("TradesFetcher");
            
            var service = new BlocktraderService(this.log, proxySettings);
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
            
            scheduler = new AlignedScheduler(DownloadTradeListsAsync, TimeSpan.FromSeconds(10), this.log);

            log.Info("TradesFetcher initializated");
        }
        
        protected override async Task RunAsync(CancellationToken token)
        {
            await scheduler.RunAsync(token).ConfigureAwait(false);
        }

        private Trade[] FilterTrades(ExchangeTitle exchange, Trade[] trades)
        {
            var log = this.log.ForContext(exchange.ToString());
            var lastId = State.LastIds[exchange][Ticker.BtcUsd];
            var newTrades = trades.Where(t => t.Id > lastId).ToArray();
            
            if (trades.Length != 0 && newTrades.Length == trades.Length)
            {
                var lostTradersCount = newTrades[0].Id - lastId;
                log.Warn($"Got {newTrades.Length}/{trades.Length} new trades (with id bigger than {lastId}). {lostTradersCount} trades are lost");
            }
            else
                log.Debug($"Got {newTrades.Length}/{trades.Length} new trades (with id bigger than {lastId})");
            
            var result = newTrades.Where(t => t.Amount > State.MinimumAmount).ToArray();
            log.Debug($"Got {result.Length}/{newTrades.Length} with amount bigger than {State.MinimumAmount}");

            if (newTrades.Length > 0)
            {
                var maxId = newTrades.Max(t => t.Id);
                log.Debug($"New last id is {maxId}");
                State.LastIds[exchange][Ticker.BtcUsd] = maxId;
            }

            return result;
        }
    }
}