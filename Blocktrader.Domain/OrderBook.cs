namespace Blocktrader.Domain
{
    public class OrderBook
    {
        public Order[] Bids { get; set; } = new Order[0];
        
        public Order[] Asks { get; set; } = new Order[0];
    }
}