namespace Nexus.Blocktrader.Models
{
    public class OrderBook
    {
        public Order[] Bids { get; }
        public Order[] Asks { get; }

        public OrderBook(Order[] bids, Order[] asks)
        {
            Bids = bids ?? new Order[0];
            Asks = asks ?? new Order[0];
        }
    }
}