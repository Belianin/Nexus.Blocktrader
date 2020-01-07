using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Blocktrader.Binance;
using Newtonsoft.Json;

namespace Blocktrader.Bitfinex
{
    public class BitfinexExchange : IExchange
    {
        private readonly WebClient web;
        
        private TimeSpan updatePeriod = TimeSpan.FromSeconds(5);
        
        private IEnumerable<OrderWithCount> bids = new List<OrderWithCount>();
        
        private IEnumerable<OrderWithCount> asks = new List<OrderWithCount>();

        public ExchangeInfo GetInfo()
        {
            return new ExchangeInfo
            {
                Bids = bids,
                Asks = asks
            };
        }

        public event EventHandler OnUpdate;

        public BitfinexExchange()
        {
            web = new WebClient();
            Task.Run(Update);
        }

        private void SetBindsAndAsks()
        {
            var response = GetOrderBook("tBTCUSD", 100);
            var orders = response.Select(r => (OrderWithCount) r);

            bids = orders.Where(o => o.Amount > 0);
            asks = orders.Where(o => o.Amount < 0).ForEach(o => o.Amount = -o.Amount);
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

        private string[][] GetOrderBook(string symbol, int limit)
        {
            var response = web.DownloadString($"https://api-pub.bitfinex.com/v2/book/{symbol}/P0?len={limit}");
            var result = JsonConvert.DeserializeObject<string[][]>(response);
            return result;
        }
    }
}