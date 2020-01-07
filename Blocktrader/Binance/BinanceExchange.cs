using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Blocktrader.Binance
{
    public class BinanceExchange
    {
        private readonly WebClient web;
        
        private TimeSpan updatePeriod = TimeSpan.FromSeconds(5);
        
        public IEnumerable<BinanceOrder> Bids { get; set; } = new List<BinanceOrder>();

        public IEnumerable<BinanceOrder> Asks { get; set; } = new List<BinanceOrder>();

        public event EventHandler OnUpdate;

        public BinanceExchange()
        {
            web = new WebClient();
            Task.Run(Update);
        }

        private void SetBindsAndAsks()
        {
            var response = GetOrderBook("BTCUSDT", 100);
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

        private OrderBookResponse GetOrderBook(string symbol, int limit)
        {
            var response = web.DownloadString($"https://api.binance.com/api/v3/depth?symbol={symbol}&limit={limit}");
            var result = JsonConvert.DeserializeObject<OrderBookResponse>(response);
            return result;
        }
    }
}