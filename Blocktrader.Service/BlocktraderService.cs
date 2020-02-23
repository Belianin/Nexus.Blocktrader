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

        private readonly IReadOnlyCollection<Ticket> tickets = new[]
        {
            Ticket.BtcUsd,
            Ticket.EthBtc,
            Ticket.EthUsd,
            Ticket.XrpBtc,
            Ticket.XrpUsd
        };

        public BlocktraderService(ILog log)
        {
            binance = new BinanceClient(log);
            bitfinex = new BitfinexClient(log);
            bitstamp = new BitstampClient(log);
        }
        
        public async Task<Timestamp> GetTimestampAsync()
        {
            var result = new Timestamp
            {
                Binance = await GetExchangeTimestampAsync(binance).ConfigureAwait(false),
                Bitfinex = await GetExchangeTimestampAsync(bitfinex).ConfigureAwait(false),
                Bitstamp = await GetExchangeTimestampAsync(bitstamp).ConfigureAwait(false),
                DateTime = DateTime.Now
            };

            return result;
        }

        private async Task<ExchangeTimestamp> GetExchangeTimestampAsync(IExchangeClient client)
        {
            var result = new ExchangeTimestamp();
            foreach (var ticket in tickets)
            {
                var tickerInfo = await client.GetTickerInfoAsync(ticket).ConfigureAwait(false);
                result.Tickers[ticket] = tickerInfo;
            }

            return result;
        }
    }
}