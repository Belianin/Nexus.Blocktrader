using System.Collections.Generic;
using Nexus.Blocktrader.Domain;

namespace Nexus.Blocktrader.Service
{
    public class ExchangeTimestamp
    {
        public Dictionary<Ticker, TickerInfo> Tickets = new Dictionary<Ticker, TickerInfo>();
    }
}