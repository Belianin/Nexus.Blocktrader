using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Exchange.Bitstamp.Models;
using Nexus.Blocktrader.Utils;

namespace Nexus.Blocktrader.Exchange.Bitstamp
{
    public class BitstampClient : BaseClient, IExchangeClient
    {
        private const string BaseUrl = "https://www.bitstamp.net/api/v2/";
        private readonly Dictionary<Ticker, string> symbols = new Dictionary<Ticker, string>
        {
            {Ticker.BtcUsd, "btcusd"},
            {Ticker.EthUsd, "ethusd"},
            {Ticker.EthBtc, "ethbtc"},
            {Ticker.XrpBtc, "xrpbtc"},
            {Ticker.XrpUsd, "xrpusd"}
        };
        
        public BitstampClient(ILogger log) : base(log)
        {
        }

        public async Task<Result<OrderBook>> GetOrderBookAsync(Ticker ticker)
        {
            var symbol = symbols[ticker];
            
            var response = await GetAsync<OrderBookResponse>($"{BaseUrl}order_book/{symbol}/");
            if (response.IsFail)
                return response.Error;

            return ParseOrderBook(response.Value);
        }

        public async Task<Result<float>> GetCurrentAveragePriceAsync(Ticker ticker)
        {
            var symbol = symbols[ticker];

            var result = await GetAsync<Dictionary<string, object>>($"{BaseUrl}ticker/{symbol}/")
                .ConfigureAwait(false);

            if (result.IsFail)
                return result.Error;
            
            
            return float.Parse((string) result.Value["last"], CultureInfo.InvariantCulture);
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