using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Nexus.Blocktrader.Exchanges.Bitstamp.Models;
using Nexus.Blocktrader.Models;
using Nexus.Core;
using Nexus.Logging;

namespace Nexus.Blocktrader.Exchanges.Bitstamp
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
        
        public BitstampClient(ILog log, ExchangeProxySettings settings = null) : base(log, settings)
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

        public async Task<Result<Trade[]>> GetLastTradesAsync(Ticker ticker)
        {
            var symbol = symbols[ticker];

            var result = await GetAsync<TradeResponse[]>($"{BaseUrl}transactions/{symbol}")
                .ConfigureAwait(false);

            if (result.IsFail)
                return result.Error;

            return result.Value.Select(ParseTrade).ToArray();
        }

        public ExchangeTitle Title => ExchangeTitle.Bitstamp;

        private Trade ParseTrade(TradeResponse response)
        {
            return new Trade(
                response.Tid,
                response.Price,
                response.Amount,
                response.Type == 1,
                response.Date.ToDateTimeFromSeconds());
        }

        private static OrderBook ParseOrderBook(OrderBookResponse response)
        {
            var bids = response.Bids.Select(ParseOrder);
            var asks = response.Asks.Select(ParseOrder);

            return new OrderBook(bids.ToArray(), asks.ToArray());
        }

        private static Order ParseOrder(string[] parameters)
        {
            return new Order(
                float.Parse(parameters[0], CultureInfo.InvariantCulture),
                float.Parse(parameters[1], CultureInfo.InvariantCulture));
        }
    }
}