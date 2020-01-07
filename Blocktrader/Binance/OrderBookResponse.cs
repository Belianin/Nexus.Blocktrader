namespace Blocktrader.Binance
{
    class OrderBookResponse
    {
        public int LastUpdateId { get; set; }
        
        public int Timestamp { get; set; }
        
        public string[][] Bids { get; set; }
        
        public string[][] Asks { get; set; }
    }
}