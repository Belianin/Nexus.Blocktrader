using System.Collections.Generic;
using Blocktrader.Domain;

namespace Blocktrader.Service
{
    public class ExchangeTimestamp
    {
        public Dictionary<Ticket, TickerInfo> Tickers = new Dictionary<Ticket, TickerInfo>();
    }
}