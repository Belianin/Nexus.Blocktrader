using System.Diagnostics.CodeAnalysis;
using Blocktrader.Domain;

namespace Blocktrader.Service
{
    public class TicketInfo
    {
        public float AveragePrice { get; }
        
        public OrderBook OrderBook { get; }

        public TicketInfo(float averagePrice, [NotNull] OrderBook orderBook)
        {
            AveragePrice = averagePrice;
            OrderBook = orderBook;
        }
    }
}