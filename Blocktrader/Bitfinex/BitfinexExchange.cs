using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace Blocktrader.Bitfinex
{
    public class BitfinexExchange : BaseExchange
    {
        private readonly WebClient web;

        private Dictionary<Ticket, string> symbols = new Dictionary<Ticket, string>
        {
            {Ticket.BtcUsd, "tBTCUSD"},
            {Ticket.EthUsd, "tETHUSD"},
            {Ticket.EthBtc, "tETHBTC"},
            {Ticket.XrpBtc, "tXRPBTC"},
            {Ticket.XrpUsd, "tXRPUSD"}
        };
        public Ticket Ticket { get; set; }

        public BitfinexExchange() : base("Bitfinex", new List<Ticket>{ Ticket.BtcUsd, Ticket.EthUsd, Ticket.EthBtc, Ticket.XrpUsd, Ticket.XrpBtc})
        {
            web = new WebClient();
        }

        public override Timestamp GetTimestamp(Ticket ticket)
        {
            try
            {
                var symbol = symbols[ticket];
                var response = GetOrderBook(symbol, 100);
                var orders = response.Select(r => (OrderWithCount) r).ToArray();
                var averagePrice = GetAveragePrice(symbol);

                var bids = orders.Where(o => o.Amount > 0);
                var asks = orders.Where(o => o.Amount < 0).ForEach(o => o.Amount = -o.Amount);

                return new Timestamp
                {
                    Date = DateTime.Now,
                    AveragePrice = averagePrice,
                    Bids = bids.ToArray(),
                    Asks = asks.ToArray()
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

        private string[][] GetOrderBook(string symbol, int limit)
        {
            var uri = $"https://api-pub.bitfinex.com/v2/book/{symbol}/P0?len={limit}";
            var response = web.DownloadString(uri);
            var result = JsonConvert.DeserializeObject<string[][]>(response);
            return result;
        }
        
        private float GetAveragePrice(string symbol)
        {
            var uri = $"https://api-pub.bitfinex.com/v2/ticker/{symbol}";
            var response = web.DownloadString(uri);
            var result = JsonConvert.DeserializeObject<float[]>(response);
            return result[6];
        }
    }
}