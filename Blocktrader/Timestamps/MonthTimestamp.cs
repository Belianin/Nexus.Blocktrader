using System;
using System.Linq;
using Nexus.Blocktrader.Models;

namespace Nexus.Blocktrader.Timestamps
{
    public class MonthTimestamp
    {
        public Ticker Ticker { get; }
        public ExchangeTitle Exchange { get; }
        public Timestamp[] Timestamps { get; }
        public int Year { get; }
        public int Month { get; }

        public MonthTimestamp(DateTime dateTime, ExchangeTitle exchange, Ticker ticker, Timestamp[] timestamps)
        {
            Year = dateTime.Year;
            Month = dateTime.Month;
            Ticker = ticker;
            Exchange = exchange;
            Timestamps = timestamps;
        }

        public Timestamp[] GetForDay(int day)
        {
            return Timestamps.Where(t => t.Date.Day == day).ToArray();
        }
    }
}