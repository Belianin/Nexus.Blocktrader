using System;
using System.Collections.Generic;
using Blocktrader.Domain;

namespace Blocktrader.Service.Files
{
    public class MonthTimestamp
    {
        public MonthTimestamp(DateTime dateTime, Ticket ticker)
        {
            DateTime = dateTime;
            Ticker = ticker;
        }

        public DateTime DateTime { get; set; }
        
        public Ticket Ticker { get; set; }
        
        public Dictionary<DateTime, Dictionary<ExchangeTitle, TicketInfo>> Info { get; set; } =
            new Dictionary<DateTime, Dictionary<ExchangeTitle, TicketInfo>>();
    }
}