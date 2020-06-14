using System.Collections.Generic;
using Nexus.Blocktrader.Models;

namespace Nexus.Blocktrader
{
    public class ExchangeTimestamp
    {
        public Dictionary<Ticker, TickerInfo> Tickets = new Dictionary<Ticker, TickerInfo>();
    }
}