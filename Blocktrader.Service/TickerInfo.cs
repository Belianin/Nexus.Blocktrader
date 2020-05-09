using System;
using System.Diagnostics.CodeAnalysis;
using Nexus.Blocktrader.Domain;

namespace Nexus.Blocktrader.Service
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