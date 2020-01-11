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

        public BitfinexExchange() : base("Bitfinex", new List<Ticket>{ Ticket.BtcUsd, Ticket.EthUsd, Ticket.EthBtc, Ticket.XrpUsd, Ticket.XrpBtc})
        {
            web = new WebClient();
        }

        public override Timestamp GetTimestamp(Ticket ticket)
        {
            var symbol = symbols[ticket];
            var response = GetOrderBook(symbol, 100);
            var orders = response.Select(r => (OrderWithCount) r).ToArray();

            var bids = orders.Where(o => o.Amount > 0);
            var asks = orders.Where(o => o.Amount < 0).ForEach(o => o.Amount = -o.Amount);

            return new Timestamp
            {
                Date = DateTime.Now,
                Bids = bids.ToArray(),
                Asks = asks.ToArray()
            };
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