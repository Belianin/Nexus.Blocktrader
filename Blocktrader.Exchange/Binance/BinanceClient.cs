using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Exchange.Binance.Models;
using Nexus.Blocktrader.Utils;
using Nexus.Logging;

namespace Nexus.Blocktrader.Exchange.Binance
{
    public class BinanceClient : BaseClient, IExchangeClient
    {
        private const string BaseUrl = "https://api.binance.com/api/v3/";
        
        private Dictionary<Ticker, string> symbols = new Dictionary<Ticker, string>
        {
            {Ticker.BtcUsd, "BTCUSDT"},
            {Ticker.EthUsd, "ETHUSDT"},
            {Ticker.EthBtc, "ETHBTC"},
            {Ticker.XrpBtc, "XRPBTC"},
            {Ticker.XrpUsd, "XRPUSDT"}
        };

        public BinanceClient(ILog log) : base(log) {}
        
        public async Task<Result<OrderBook>> GetOrderBookAsync(Ticker ticker)
        {
            var symbol = symbols[ticker];
            
            var result = await GetAsync<OrderBookResponse>($"{BaseUrl}depth?symbol={symbol}&limit={5000}")
                .ConfigureAwait(false);

            if (result.IsFail)
            {
                return result.Error;
            }

            return ParseOrderBook(result.Value);
        }

        public async Task<Result<float>> GetCurrentAveragePriceAsync(Ticker ticker)
        {
            var symbol = symbols[ticker];
            
            var result = await GetAsync<AveragePriceResponse>($"{BaseUrl}avgPrice?symbol={symbol}")
                .ConfigureAwait(false);

            if (result.IsFail)
                return $"Couldn't get average price: {result.Error}";

            if (!float.TryParse(result.Value.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
                return $"Couldn't parse average price";

            return price;
        }

        public async Task<Result<Trade[]>> GetLastTradesAsync(Ticker ticker)
        {
            var symbol = symbols[ticker];

            var result = await GetAsync<TradeResponse[]>($"{BaseUrl}trades?symbol={symbol}&limit={1000}")
                .ConfigureAwait(false);

            if (result.IsFail)
                return $"Couldn't get trades list: {result.Error}";

            return result.Value.Select(ParseTrade).ToArray();
        }

        private static Trade ParseTrade(TradeResponse response)
        {
            return new Trade(
                response.Id,
                response.Price,
                float.Parse(response.Qty, CultureInfo.InvariantCulture),
                !response.IsBuyerMaker,
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddMilliseconds(response.Time));
        }

        private static OrderBook ParseOrderBook(OrderBookResponse response)
        {
            var bids = response.Bids.Select(ParseOrder);
            var asks = response.Asks.Select(ParseOrder);

            return new OrderBook(bids.ToArray(), asks.ToArray());
        }

        private static Order ParseOrder(string[] parameters)
        {
            return new Order
            {
                Price = float.Parse(parameters[0], CultureInfo.InvariantCulture),
                Amount = float.Parse(parameters[1], CultureInfo.InvariantCulture)
            };
        }
    }
}