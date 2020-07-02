using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nexus.Blocktrader.Exchanges;
using Nexus.Blocktrader.Exchanges.Binance;
using Nexus.Blocktrader.Exchanges.Bitfinex;
using Nexus.Blocktrader.Exchanges.Bitstamp;
using Nexus.Blocktrader.Models;
using Nexus.Logging;

namespace Nexus.Blocktrader
{
    public class BlocktraderService
    {
        private readonly BinanceClient binance;
        private readonly BitfinexClient bitfinex;
        private readonly BitstampClient bitstamp;
        private readonly Dictionary<ExchangeTitle, IExchangeClient> exchangeClients;

        public BlocktraderService(ILog log, ExchangeProxySettings settings)
        {
            binance = new BinanceClient(log);
            bitfinex = new BitfinexClient(log);
            bitstamp = new BitstampClient(log, settings.Host, settings.Port);
            
            exchangeClients = new Dictionary<ExchangeTitle, IExchangeClient>
            {
                [ExchangeTitle.Binance] = binance,
                [ExchangeTitle.Bitfinex] = bitfinex,
                [ExchangeTitle.Bitstamp] = bitstamp
            };
        }
        
        public async Task<CommonTimestamp> GetCurrentTimestampAsync()
        {
            var binanceTask = Task.Run(async () => await GetExchangeTimestampAsync(binance));
            var bitfinexTask = Task.Run(async () => await GetExchangeTimestampAsync(bitfinex));
            var bitstampTask = Task.Run(async () => await GetExchangeTimestampAsync(bitstamp));

            var tasks = new[] {binanceTask, bitfinexTask, bitstampTask};
            
            await Task.WhenAll(tasks);


            var result = new CommonTimestamp
            {
                Exchanges = new Dictionary<ExchangeTitle, ExchangeTimestamp>
                {
                    [ExchangeTitle.Binance] = binanceTask.Result,
                    [ExchangeTitle.Bitfinex] = bitfinexTask.Result,
                    [ExchangeTitle.Bitstamp] = bitstampTask.Result,
                },
                DateTime = DateTime.Now
            };

            return result;
        }

        public async Task<Trade[]> GetCurrentTradeListsAsync(ExchangeTitle exchange, Ticker ticker)
        {
            var result = await exchangeClients[exchange].GetLastTradesAsync(ticker).ConfigureAwait(false);

            return result.IsSuccess ? result.Value : new Trade[0];
        }
        
        public async Task<Dictionary<ExchangeTitle, Trade[]>> GetCurrentTradeListsAsync()
        {
            var binanceTask = Task.Run(async () => await binance.GetLastTradesAsync(Ticker.BtcUsd));
            var bitfinexTask = Task.Run(async () => await bitfinex.GetLastTradesAsync(Ticker.BtcUsd));
            var bitstampTask = Task.Run(async () => await bitstamp.GetLastTradesAsync(Ticker.BtcUsd));

            var tasks = new[] {binanceTask, bitfinexTask, bitstampTask };
            await Task.WhenAll(tasks);


            var result = new Dictionary<ExchangeTitle, Trade[]>
            {
                [ExchangeTitle.Binance] = binanceTask.Result.IsSuccess ? binanceTask.Result.Value : new Trade[0],
                [ExchangeTitle.Bitfinex] = bitfinexTask.Result.IsSuccess ? bitfinexTask.Result.Value : new Trade[0],
                [ExchangeTitle.Bitstamp] = bitstampTask.Result.IsSuccess ? bitstampTask.Result.Value : new Trade[0],
            };

            return result;
        }

        private async Task<ExchangeTimestamp> GetExchangeTimestampAsync(IExchangeClient client)
        {
            var result = new ExchangeTimestamp();
            foreach (var ticket in (Ticker[]) Enum.GetValues(typeof(Ticker)))
            {
                var tickerInfo = await client.GetTickerInfoAsync(ticket).ConfigureAwait(false);
                result.Tickets[ticket] = tickerInfo;
            }

            return result;
        }
    }
}