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
    public class BinanceExchange : IExchange
    {
        private readonly WebClient web;
        
        private TimeSpan updatePeriod = TimeSpan.FromSeconds(5);
        
        private IEnumerable<Order> bids = new List<Order>();

        private IEnumerable<Order> asks = new List<Order>();

        public ExchangeInfo GetInfo()
        {
            return new ExchangeInfo
            {
                Bids = bids,
                Asks = asks,
            };
        }

        public event EventHandler OnUpdate;

        public BinanceExchange()
        {
            web = new WebClient();
            Task.Run(Update);
        }

        private void SetBindsAndAsks()
        {
            var response = GetOrderBook("BTCUSDT", 100);
            bids = response.Bids.Select(ParseOrder);
            asks = response.Asks.Select(ParseOrder);
        }
        
        private Order ParseOrder(string[] parameters)
        {
            return new Order
            {
                Price = decimal.Parse(parameters[0], CultureInfo.InvariantCulture),
                Amount = decimal.Parse(parameters[1], CultureInfo.InvariantCulture)
            };
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