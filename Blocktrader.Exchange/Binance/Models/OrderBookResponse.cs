namespace Blocktrader.Exchange.Binance.Models
{
    internal class OrderBookResponse
    {
        public int LastUpdateId { get; set; }
        
        public int Timestamp { get; set; }
        
        public string[][] Bids { get; set; }
        
        public string[][] Asks { get; set; }
    }
}