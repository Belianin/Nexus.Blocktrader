using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Exchange.Bitfinex.Models;
using Nexus.Blocktrader.Utils;
using Nexus.Blocktrader.Utils.Logging;

namespace Nexus.Blocktrader.Exchange.Bitfinex
{
    public class BitfinexClient : BaseClient, IExchangeClient
    {
        private const string BaseUrl = "https://api-pub.bitfinex.com/v2/";
        private Dictionary<Ticker, string> symbols = new Dictionary<Ticker, string>
        {
            {Ticker.BtcUsd, "tBTCUSD"},
            {Ticker.EthUsd, "tETHUSD"},
            {Ticker.EthBtc, "tETHBTC"},
            {Ticker.XrpBtc, "tXRPBTC"},
            {Ticker.XrpUsd, "tXRPUSD"}
        };
        
        public BitfinexClient(ILog log) : base(log)
        {
        }

        public async Task<Result<OrderBook>> GetOrderBookAsync(Ticker ticker)
        {
            var symbol = symbols[ticker];
            var maxPrice = 0f; // for asks
            var minPrice = 0f; // for bids
            var bids = new List<Order>();
            var asks = new List<Order>();

            foreach (var precision in new [] {"P0", "P1", "P2", "P3", "P4"})
            {
                var bookResult = await GetOrderBookWithPrecision(symbol, precision).ConfigureAwait(false);

                // Задержка чтобы Bitfinex не прибанил
                Task.Delay(5 * 1000).Wait();
                
                if (bookResult.IsFail)
                    continue;

                var book = bookResult.Value;
                bids.AddRange(book.Bids.Where(b => b.Price < minPrice));
                asks.AddRange(book.Asks.Where(b => b.Price > maxPrice));

                // Не оптимально, но нам пофиг (oldbamboe)
                minPrice = book.Bids.Select(b => b.Price).Min();
                maxPrice = book.Asks.Select(b => b.Price).Max();
            }
            
            return new OrderBook(bids.ToArray(), asks.ToArray());
        }

        public async Task<Result<float>> GetCurrentAveragePriceAsync(Ticker ticker)
        {
            var symbol = symbols[ticker];

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

            return new OrderBook(bids.Cast<Order>().ToArray(), asks.Cast<Order>().ToArray());
        }

        private async Task<Result<OrderBook>> GetOrderBookWithPrecision(string symbol, string precision)
        {
            var result = await GetAsync<string[][]>($"{BaseUrl}book/{symbol}/{precision}?len={100}")
                .ConfigureAwait(false);

            if (result.IsFail)
                return result.Error;
            
            return ParseOrderBook(result.Value);
        } 
    }
}