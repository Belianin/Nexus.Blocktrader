using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blocktrader.Utils;
using Blocktrader.Utils.Logging;    

namespace Blocktrader.Bitfinex
{
    public class BitfinexClient : BaseClient, IExchangeClient
    {
        private const string BaseUrl = "https://api-pub.bitfinex.com/v2/";
        private Dictionary<Ticket, string> symbols = new Dictionary<Ticket, string>
        {
            {Ticket.BtcUsd, "tBTCUSD"},
            {Ticket.EthUsd, "tETHUSD"},
            {Ticket.EthBtc, "tETHBTC"},
            {Ticket.XrpBtc, "tXRPBTC"},
            {Ticket.XrpUsd, "tXRPUSD"}
        };
        
        public BitfinexClient(ILog log) : base(log)
        {
        }

        public async Task<Result<OrderBook>> GetOrderBookAsync(Ticket ticket)
        {
            var symbol = symbols[ticket];

            var result = await GetAsync<string[][]>($"{BaseUrl}book/{symbol}/P0?len={100}")
                .ConfigureAwait(false);

            if (result.IsFail)
                return result.Error;
            
            return ParseOrderBook(result.Value);
        }

        public async Task<Result<float>> GetCurrentAveragePriceAsync(Ticket ticket)
        {
            var symbol = symbols[ticket];

            var result = await GetAsync<float[]>($"{BaseUrl}ticker/{symbol}").ConfigureAwait(false);

            if (result.IsFail)
                return result.Error;
            
            return result.Value[6];
        }

        private OrderBook ParseOrderBook(string[][] response)
        {
            var orders = response.Select(r => (OrderWithCount) r).ToArray();

            var bids = orders.Where(o => o.Amount > 0);
            var asks = orders.Where(o => o.Amount < 0).ForEach(o => o.Amount = -o.Amount);

            return new OrderBook
            {
                Asks = asks.ToArray(),
                Bids = bids.ToArray()
            };
        }
    }
}