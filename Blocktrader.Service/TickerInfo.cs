using Blocktrader.Domain;

namespace Blocktrader.Service
{
    public class TickerInfo
    {
        public float AveragePrice { get; set; }
        
        public OrderBook OrderBook { get; set; }
    }
}