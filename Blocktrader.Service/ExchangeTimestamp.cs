using System.Collections.Generic;
using Blocktrader.Domain;

namespace Blocktrader.Service
{
    public class ExchangeTimestamp
    {
        public Dictionary<Ticket, TicketInfo> Tickets = new Dictionary<Ticket, TicketInfo>();
    }
}