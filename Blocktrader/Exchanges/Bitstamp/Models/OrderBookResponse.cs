namespace Nexus.Blocktrader.Exchanges.Bitstamp.Models
{
    internal class OrderBookResponse
    {
        public long LastUpdateId { get; set; }
        
        public int Timestamp { get; set; }
        
        public string[][] Bids { get; set; }
        
        public string[][] Asks { get; set; }
    }
}