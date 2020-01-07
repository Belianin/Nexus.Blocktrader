using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        private FileStream GetWriter()
        {
            var filename = $"bitstamp_{Ticket}_{DateTime.Now.ToString("MMM-yyyy", new CultureInfo("en_US"))}";

            return File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        }

        private void SetBindsAndAsks()
        {
            if (!tickets.TryGetValue(Ticket, out var symbol))
                return;
            var writer = GetWriter();
            var response = GetOrderBook(symbol);
            bids = response.Bids.Select(ParseOrder);
            asks = response.Asks.Select(ParseOrder);

            var timestamp = new Timestamp
            {
                Date = DateTime.UtcNow,
                Orders = bids.ToArray()
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

        private OrderBookResponse GetOrderBook(string symbol)
        {
            var response = web.DownloadString($"https://bitstamp.net/api/v2/order_book/{symbol}/");
            var result = JsonConvert.DeserializeObject<OrderBookResponse>(response);
            return result;
        }
    }
}