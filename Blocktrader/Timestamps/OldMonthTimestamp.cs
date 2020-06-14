using System;
using System.Collections.Generic;
using Nexus.Blocktrader.Models;

namespace Nexus.Blocktrader.Timestamps
{
    public class OldMonthTimestamp
    {
        public OldMonthTimestamp(DateTime dateTime, Ticker ticker)
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