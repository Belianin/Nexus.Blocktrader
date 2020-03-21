using System;
using System.Collections.Generic;
using Blocktrader.Domain;

namespace Blocktrader.Service.Files
{
    public class MonthTimestamp
    {
        public MonthTimestamp(DateTime dateTime, Ticker ticker)
        {
            DateTime = dateTime;
            Ticker = ticker;
        }

        public DateTime DateTime { get; set; }
        
        public Ticker Ticker { get; set; }
        
        public Dictionary<DateTime, Dictionary<ExchangeTitle, TickerInfo>> Info { get; set; } =
            new Dictionary<DateTime, Dictionary<ExchangeTitle, TickerInfo>>();
    }
}