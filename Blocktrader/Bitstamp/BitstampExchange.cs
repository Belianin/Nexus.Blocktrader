using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blocktrader.Binance;
using Newtonsoft.Json;

namespace Blocktrader.Bitstamp
{
    public class BitstampExchange : BaseExchange, IExchange
    {
        private readonly WebClient web;

        private readonly HttpClient client;
        
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
            foreach (var ticket in tickets.Keys) 
                SetBindsAndAsks(ticket);
            OnUpdate?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnUpdate;

        public BitstampExchange() : base("Bitstamp")
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

        private void SetBindsAndAsks(Ticket ticket)
        {
            using var writer = GetWriter(ticket);
            var response = GetOrderBook(ticket);
            bids = response.Bids.Select(ParseOrder);
            asks = response.Asks.Select(ParseOrder);

            var timestamp = new Timestamp
            {
                Date = DateTime.UtcNow,
                Bids = bids.ToArray(),
                Asks = asks.ToArray()
            };
            
            writer.Write(timestamp.ToBytes());
        }

        private Order ParseOrder(string[] parameters)
        {
            return new Order
            {
                Price = float.Parse(parameters[0], CultureInfo.InvariantCulture),
                Amount = float.Parse(parameters[1], CultureInfo.InvariantCulture)
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

        private OrderBookResponse GetOrderBook(Ticket ticket)
        {
            var symbol = tickets[ticket];
            var response = web.DownloadString($"https://bitstamp.net/api/v2/order_book/{symbol}/");
            var result = JsonConvert.DeserializeObject<OrderBookResponse>(response);
            return result;
        }

        protected override Timestamp GetTimestamp(Ticket ticket)
        {
            var symbol = tickets[ticket];
            var response = web.DownloadString($"https://bitstamp.net/api/v2/order_book/{symbol}/");
            var book = JsonConvert.DeserializeObject<OrderBookResponse>(response);
            
            var bids = book.Bids.Select(ParseOrder);
            var asks = book.Asks.Select(ParseOrder);

            return new Timestamp
            {
                Date = DateTime.UtcNow,
                Bids = bids.ToArray(),
                Asks = asks.ToArray()
            };
        }
    }
}