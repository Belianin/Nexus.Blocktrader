using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blocktrader.Domain;
using Blocktrader.Exchange;
using Blocktrader.Exchange.Binance;
using Blocktrader.Exchange.Bitfinex;
using Blocktrader.Exchange.Bitstamp;
using Blocktrader.Utils.Logging;

namespace Blocktrader.Service
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
            var result = new CommonTimestamp
            {
                Exchanges = new Dictionary<ExchangeTitle, ExchangeTimestamp>
                {
                    [ExchangeTitle.Binance] = await GetExchangeTimestampAsync(binance).ConfigureAwait(false),
                    [ExchangeTitle.Bitfinex] = await GetExchangeTimestampAsync(bitfinex).ConfigureAwait(false),
                    [ExchangeTitle.Bitstamp] = await GetExchangeTimestampAsync(bitstamp).ConfigureAwait(false),
                },
                DateTime = DateTime.Now
            };

            return result;
        }

        private async Task<ExchangeTimestamp> GetExchangeTimestampAsync(IExchangeClient client)
        {
            var result = new ExchangeTimestamp();
            foreach (var ticket in (Ticket[]) Enum.GetValues(typeof(Ticket)))
            {
                var tickerInfo = await client.GetTickerInfoAsync(ticket).ConfigureAwait(false);
                result.Tickets[ticket] = tickerInfo;
            }

            return result;
        }
    }
}