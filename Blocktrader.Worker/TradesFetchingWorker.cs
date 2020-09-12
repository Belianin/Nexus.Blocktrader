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
        private readonly ICollection<FlexScheduler> schedulers;
        private readonly Dictionary<ExchangeTitle, Queue<Trade[]>> queues = new Dictionary<ExchangeTitle, Queue<Trade[]>>
        {
            [ExchangeTitle.Binance] = new Queue<Trade[]>(),
            [ExchangeTitle.Bitfinex] = new Queue<Trade[]>(),
            [ExchangeTitle.Bitstamp] = new Queue<Trade[]>()
        };

        public TradesFetchingWorker(ILog log)
        {
            var proxySettings = File.Exists("proxy.settings.json")
                ? JsonConvert.DeserializeObject<ExchangeProxySettings>(File.ReadAllText("proxy.settings.json"))
                : new ExchangeProxySettings();
            this.log = log.ForContext("TradesFetcher");

            schedulers = GetSchedulers(proxySettings).ToArray();

            log.Info("TradesFetcher initializated");
        }

        private IEnumerable<FlexScheduler> GetSchedulers(ExchangeProxySettings proxySettings)
        {
            var service = new BlocktraderService(log, proxySettings);
            foreach (var exchange in new [] {ExchangeTitle.Binance, ExchangeTitle.Bitfinex, ExchangeTitle.Bitstamp})
            {
                var thisLog = log.ForContext(exchange.ToString());
                async Task<TimeSpan> DownloadTradesAsync()
                {
                    var trades = await service.GetCurrentTradeListsAsync(exchange, Ticker.BtcUsd)
                        .ConfigureAwait(false);

                    var (filtered, timeSpan) = FilterTrades(exchange, trades, thisLog);
                    
                    queues[exchange].Enqueue(filtered);

                    return timeSpan;
                }
                
                yield return new FlexScheduler(DownloadTradesAsync, thisLog);
            }
        }

        private async Task WriteAsync(CancellationToken token)
        {
            var manager = new FileTradesManager(log);
            while (!token.IsCancellationRequested)
            {
                foreach (var exchange in new[] {ExchangeTitle.Binance, ExchangeTitle.Bitfinex, ExchangeTitle.Bitstamp})
                {
                    while (queues[exchange].TryDequeue(out var trades))
                        await manager.WriteAsync(exchange, trades, Ticker.BtcUsd).ConfigureAwait(false);
                }

                await Task.Delay(5 * 1000, token).ConfigureAwait(false);
            }
        }
        
        protected override async Task RunAsync(CancellationToken token)
        {
            var tasks = schedulers
                .Select(s => s.RunAsync(token))
                .Concat(new[] {WriteAsync(token)})
                .ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private (Trade[], TimeSpan) FilterTrades(ExchangeTitle exchange, Trade[] trades, ILog log)
        {
            var lastId = State.LastIds[exchange][Ticker.BtcUsd];
            var newTrades = trades.Where(t => t.Id > lastId).ToArray();

            if (trades.Length != 0 && newTrades.Length == trades.Length)
            {
                var lostTradesCount = newTrades[0].Id - lastId;
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