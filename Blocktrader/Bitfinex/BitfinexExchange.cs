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
        
        private Dictionary<Ticket, string> tickets = new Dictionary<Ticket, string>
        {
            {Ticket.BtcUsd, "tBTCUSD"},
            {Ticket.EthUsd, "tETHUSD"},
            {Ticket.EthBtc, "tETHBTC"},
            {Ticket.XrpBtc, "tXRPBTC"},
            {Ticket.XrpUsd, "tXRPUSD"}
        };

        public Ticket Ticket { get; set; }

        public ExchangeInfo GetInfo()
        {
            return new ExchangeInfo
            {
                Bids = bids,
                Asks = asks
            };
        }

        public void ForceUpdate()
        {
            SetBindsAndAsks();
            OnUpdate?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnUpdate;

        public BitfinexExchange()
        {
            web = new WebClient();
            Task.Run(Update);
        }

        private void SetBindsAndAsks()
        {
            if (!tickets.TryGetValue(Ticket, out var symbol))
                return;
            var response = GetOrderBook(symbol, 100);
            var orders = response.Select(r => (OrderWithCount) r);

            bids = orders.Where(o => o.Amount > 0);
            asks = orders.Where(o => o.Amount < 0).ForEach(o => o.Amount = -o.Amount);
        }

        private void Update()
        {
            while (true)
            {
                ForceUpdate();
                Thread.Sleep(updatePeriod);
            }
        }

        private string[][] GetOrderBook(string symbol, int limit)
        {
            var uri = $"https://api-pub.bitfinex.com/v2/book/{symbol}/P0?len={limit}";
            Console.WriteLine(uri);
            var response = web.DownloadString(uri);
            var result = JsonConvert.DeserializeObject<string[][]>(response);
            return result;
        }
    }
}