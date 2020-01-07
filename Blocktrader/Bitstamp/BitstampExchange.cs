using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Blocktrader.Binance;
using Newtonsoft.Json;

namespace Blocktrader.Bitstamp
{
    public class BitstampExchange : IExchange
    {
        private readonly WebClient web;
        
        private TimeSpan updatePeriod = TimeSpan.FromSeconds(5);
        
        private IEnumerable<Order> bids = new List<Order>();

        private IEnumerable<Order> asks = new List<Order>();
        
        private Dictionary<Ticket, string> tickets = new Dictionary<Ticket, string>
        {
            {Ticket.BtcUsd, "btcusd"},
            {Ticket.EthUsd, "ethusd"},
            {Ticket.EthBtc, "ethbtc"},
            {Ticket.XrpBtc, "xrpbtc"},
            {Ticket.XrpUsd, "xrpusd"}
        };

        public Ticket Ticket { get; set; }

        public void ForceUpdate()
        {
            SetBindsAndAsks();
            OnUpdate?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnUpdate;

        public BitstampExchange()
        {
            web = new WebClient();
            Task.Run(Update);
        }

        public ExchangeInfo GetInfo()
        {
            return new ExchangeInfo
            {
                Asks = asks,
                Bids = bids
            };
        }

        private void SetBindsAndAsks()
        {
            if (!tickets.TryGetValue(Ticket, out var symbol))
                return;
            var response = GetOrderBook(symbol);
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
                ForceUpdate();
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