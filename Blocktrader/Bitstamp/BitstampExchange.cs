using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Blocktrader.Binance;
using Newtonsoft.Json;

namespace Blocktrader.Bitstamp
{
    public class BitstampExchange
    {
        private readonly WebClient web;
        
        private TimeSpan updatePeriod = TimeSpan.FromSeconds(5);
        
        public IEnumerable<BinanceOrder> Bids { get; set; } = new List<BinanceOrder>();

        public IEnumerable<BinanceOrder> Asks { get; set; } = new List<BinanceOrder>();

        public event EventHandler OnUpdate;

        public BitstampExchange()
        {
            web = new WebClient();
            Task.Run(Update);
        }

        private void SetBindsAndAsks()
        {
            var response = GetOrderBook("btcusd");
            Bids = response.Bids.Select(b => (BinanceOrder) b);
            Asks = response.Asks.Select(a => (BinanceOrder) a);
        }

        private void Update()
        {
            while (true)
            {
                SetBindsAndAsks();
                OnUpdate?.Invoke(this, EventArgs.Empty);
                Thread.Sleep(updatePeriod);
            }
        }

        private OrderBookResponse GetOrderBook(string symbol)
        {
            var response = web.DownloadString($"https://bitstamp.net/api/order_book/{symbol}");
            var result = JsonConvert.DeserializeObject<OrderBookResponse>(response);
            return result;
        }
    }
}