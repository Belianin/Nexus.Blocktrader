using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blocktrader.Utils;
using Blocktrader.Utils.Logging;

namespace Blocktrader.Bitstamp
{
    public class BitstampClient : BaseClient, IExchangeClient
    {
        private const string BaseUrl = "https://www.bitstamp.net/api/v2/";
        private readonly Dictionary<Ticket, string> symbols = new Dictionary<Ticket, string>
        {
            {Ticket.BtcUsd, "btcusd"},
            {Ticket.EthUsd, "ethusd"},
            {Ticket.EthBtc, "ethbtc"},
            {Ticket.XrpBtc, "xrpbtc"},
            {Ticket.XrpUsd, "xrpusd"}
        };
        
        public BitstampClient(ILog log) : base(log)
        {
        }

        public async Task<Result<OrderBook>> GetOrderBookAsync(Ticket ticket)
        {
            var symbol = symbols[ticket];
            
            var response = await GetAsync<OrderBookResponse>($"{BaseUrl}order_book/{symbol}/");
            if (response.IsFail)
                return response.Error;

            return ParseOrderBook(response.Value);
        }

        public async Task<Result<float>> GetCurrentAveragePriceAsync(Ticket ticket)
        {
            var symbol = symbols[ticket];

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
            
            return new OrderBook
            {
                Asks = asks.ToArray(),
                Bids = bids.ToArray()
            };
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