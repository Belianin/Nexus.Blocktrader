using System;
using System.Diagnostics.CodeAnalysis;

namespace Nexus.Blocktrader.Models
{
    public class TickerInfo
    {
        public float AveragePrice { get; }
        
        public OrderBook OrderBook { get; }
        
        public DateTime DateTime { get; }

        public TickerInfo(float averagePrice, [NotNull] OrderBook orderBook, DateTime dateTime)
        {
            AveragePrice = averagePrice;
            DateTime = dateTime;
            OrderBook = orderBook ?? new OrderBook(null, null);
        }
    }
}