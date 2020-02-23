using Blocktrader.Domain;

namespace Blocktrader.Service
{
    public class TicketInfo
    {
        public float AveragePrice { get; set; }
        
        public OrderBook OrderBook { get; set; }
    }
}