using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Exchange;
using Nexus.Blocktrader.Exchange.Binance;
using Nexus.Blocktrader.Exchange.Bitfinex;
using Nexus.Blocktrader.Exchange.Bitstamp;
using Nexus.Logging;

namespace Nexus.Blocktrader.Service
{
    public class BlocktraderService
    {
        private readonly BinanceClient binance;
        private readonly BitfinexClient bitfinex;
        private readonly BitstampClient bitstamp;

        public BlocktraderService(ILog log)
        {
            binance = new BinanceClient(log);
            bitfinex = new BitfinexClient(log);
            bitstamp = new BitstampClient(log);
        }
        
        public async Task<CommonTimestamp> GetCurrentTimestampAsync()
        {
            var binanceTask = Task.Run(async () => await GetExchangeTimestampAsync(binance));
            var bitfinexTask = Task.Run(async () => await GetExchangeTimestampAsync(bitfinex));
            var bitstampTask = Task.Run(async () => await GetExchangeTimestampAsync(bitstamp));

            var tasks = new[] {binanceTask, bitfinexTask, bitstampTask };
            Task.WaitAll(tasks); // or WhenAll ?


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