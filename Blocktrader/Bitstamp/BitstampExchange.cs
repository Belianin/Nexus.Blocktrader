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
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace Blocktrader.Bitstamp
{
    public class BitstampExchange : BaseExchange
    {
        private readonly WebClient web;

        private Dictionary<Ticket, string> symbols = new Dictionary<Ticket, string>
        {
            {Ticket.BtcUsd, "btcusd"},
            {Ticket.EthUsd, "ethusd"},
            {Ticket.EthBtc, "ethbtc"},
            {Ticket.XrpBtc, "xrpbtc"},
            {Ticket.XrpUsd, "xrpusd"}
        };
        public Ticket Ticket { get; set; }

        public BitstampExchange() : base("Bitstamp", new List<Ticket>{ Ticket.BtcUsd, Ticket.EthUsd, Ticket.EthBtc, Ticket.XrpUsd, Ticket.XrpBtc})
        {
            web = new WebClient();
        }
        
        public override Timestamp GetTimestamp(Ticket ticket)
        {
            try
            {
                var symbol = symbols[ticket];
                var response = web.DownloadString($"https://bitstamp.net/api/v2/order_book/{symbol}/");
                var book = JsonConvert.DeserializeObject<OrderBookResponse>(response);
            
                var tickerResponse = web.DownloadString($"https://www.bitstamp.net/api/v2/ticker/{symbol}/");
                var ticker = JsonConvert.DeserializeObject<Dictionary<string, object>>(tickerResponse);
                var averagePrice = float.Parse((string) ticker["last"], CultureInfo.InvariantCulture);
            
                var bids = book.Bids.Select(ParseOrder);
                var asks = book.Asks.Select(ParseOrder);

                return new Timestamp
                {
                    Date = DateTime.UtcNow,
                    AveragePrice = averagePrice,
                    Bids = bids.ToArray(),
                    Asks = asks.ToArray()
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private Order ParseOrder(string[] parameters)
        {
            return new Order
            {
                Price = float.Parse(parameters[0], CultureInfo.InvariantCulture),
                Amount = float.Parse(parameters[1], CultureInfo.InvariantCulture)
            };
        }
    }
}