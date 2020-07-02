using System;
using System.Collections.Generic;
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
        private readonly ICollection<FlexScheduler> schedulers = new List<FlexScheduler>();

        public TradesFetchingWorker(ILog log)
        {
            var proxySettings = File.Exists("proxy.settings.json")
                ? JsonConvert.DeserializeObject<ExchangeProxySettings>(File.ReadAllText("proxy.settings.json"))
                : new ExchangeProxySettings();
            this.log = log.ForContext("TradesFetcher");
            
            var service = new BlocktraderService(this.log, proxySettings);
            var manager = new FileTradesManager(this.log);

            foreach (var exchange in new [] {ExchangeTitle.Binance, ExchangeTitle.Bitfinex, ExchangeTitle.Bitstamp})
            {
                async Task<TimeSpan> DownloadTradesAsync()
                {
                    var trades = await service.GetCurrentTradeListsAsync(exchange, Ticker.BtcUsd)
                        .ConfigureAwait(false);

                    var (filtered, timeSpan) = FilterTrades(exchange, trades);

                    await manager.WriteAsync(exchange, filtered, Ticker.BtcUsd).ConfigureAwait(false);

                    return timeSpan;
                }
                
                schedulers.Add(new FlexScheduler(DownloadTradesAsync, log));
            }

            log.Info("TradesFetcher initializated");
        }
        
        protected override async Task RunAsync(CancellationToken token)
        {
            await Task.WhenAny(schedulers.Select(s => s.RunAsync(token))).ConfigureAwait(false);
        }

        private (Trade[], TimeSpan) FilterTrades(ExchangeTitle exchange, Trade[] trades)
        {
            var log = this.log.ForContext(exchange.ToString());
            var lastId = State.LastIds[exchange][Ticker.BtcUsd];
            var newTrades = trades.Where(t => t.Id > lastId).ToArray();

            var lostTradesCount = 0;
            if (trades.Length != 0 && newTrades.Length == trades.Length)
            {
                lostTradesCount = newTrades[0].Id - lastId;
                log.Warn($"Got {newTrades.Length}/{trades.Length} new trades (with id bigger than {lastId}). {lostTradesCount} trades are lost");
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

            if (trades.Length == 0)
                return (result, TimeSpan.FromSeconds(10));
            return (result, TimeSpan.FromSeconds(10 * ((trades.Length - newTrades.Length) / (double) trades.Length)));
        }
    }
}