using System.Collections.Generic;
using Blocktrader.Domain;

namespace Blocktrader.Service
{
    public class ExchangeTimestamp
    {
        public Dictionary<Ticket, TicketInfo> Tickers = new Dictionary<Ticket, TicketInfo>();
    }
}