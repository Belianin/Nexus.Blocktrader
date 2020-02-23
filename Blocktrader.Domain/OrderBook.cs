namespace Blocktrader.Domain
{
    public class OrderBook
    {
        public Order[] Bids { get; set; }
        
        public Order[] Asks { get; set; }
    }
}