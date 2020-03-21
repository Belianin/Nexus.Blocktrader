using System.Collections.Generic;
using Blocktrader.Domain;

namespace Blocktrader.Service
{
    public class ExchangeTimestamp
    {
        public Dictionary<Ticker, TickerInfo> Tickets = new Dictionary<Ticker, TickerInfo>();
    }
}