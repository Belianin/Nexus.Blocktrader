using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Blocktrader.Binance
{
    public class BinanceExchange : BaseExchange
    {
        private readonly WebClient web;
        
        private TimeSpan updatePeriod = TimeSpan.FromSeconds(30);
        
        private Dictionary<Ticket, string> symbols = new Dictionary<Ticket, string>
        {
            {Ticket.BtcUsd, "BTCUSDT"},
            {Ticket.EthUsd, "ETHUSDT"},
            {Ticket.EthBtc, "ETHBTC"},
            {Ticket.XrpBtc, "XRPBTC"},
            {Ticket.XrpUsd, "XRPUSDT"}
        };
        public Ticket Ticket { get; set; }

        public BinanceExchange() : base("Binance", new List<Ticket>{ Ticket.BtcUsd, Ticket.EthUsd, Ticket.EthBtc, Ticket.XrpUsd, Ticket.XrpBtc})
        {
            web = new WebClient();
        }
        
        public override Timestamp GetTimestamp(Ticket ticket)
        {
            try
            {
                var symbol = symbols[ticket];
                var response = GetOrderBook(symbol, 5000);
                var averagePrice = GetAveragePrice(symbol);
                var bids = response.Bids.Select(ParseOrder);
                var asks = response.Asks.Select(ParseOrder);

                return new Timestamp
                {
                    Date = DateTime.Now,
                    AveragePrice = averagePrice,
                    Bids = bids.ToArray(),
                    Asks = asks.ToArray(),
                };
            }
            catch (Exception e)
            {
                return new Timestamp
                {
                    Date = DateTime.Now,
                    AveragePrice = 0,
                    Asks = new Order[0],
                    Bids = new Order[0],
                };
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

        private OrderBookResponse GetOrderBook(string symbol, int limit)
        {
            var response = web.DownloadString($"https://api.binance.com/api/v3/depth?symbol={symbol}&limit={limit}");
            var result = JsonConvert.DeserializeObject<OrderBookResponse>(response);
            return result;
        }

        private float GetAveragePrice(string symbol)
        {
            var response = web.DownloadString($"https://api.binance.com/api/v3/avgPrice?symbol={symbol}");
            Console.WriteLine(response);
            var result = JsonConvert.DeserializeObject<AveragePriceResponse>(response);
            return float.Parse(result.Price, CultureInfo.InvariantCulture);
        }
    }
}