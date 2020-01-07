using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Blocktrader.Binance;
using Newtonsoft.Json;

namespace Blocktrader.Bitfinex
{
    public class BitfinexExchange
    {
        private readonly WebClient web;
        
        private TimeSpan updatePeriod = TimeSpan.FromSeconds(5);
        
        public IEnumerable<BitfinexOrder> Orders { get; set; } = new List<BitfinexOrder>();
        
        public ObservableCollection<BitfinexOrder> Orders2 { get; set; } = new ObservableCollection<BitfinexOrder>();

        public event EventHandler OnUpdate;

        public BitfinexExchange()
        {
            web = new WebClient();
            Task.Run(Update);
        }

        private void SetBindsAndAsks()
        {
            var response = GetOrderBook("tBTCUSD", 100);
            Orders = response.Select(r => (BitfinexOrder) r);
        }

        private void Update()
        {
            while (true)
            {
                SetBindsAndAsks();
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