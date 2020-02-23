using System;

namespace Blocktrader.Bitstamp
{
    [Obsolete("Проверить на лишние поля")]
    internal class OrderBookResponse
    {
        public int LastUpdateId { get; set; }
        
        public int Timestamp { get; set; }
        
        public string[][] Bids { get; set; }
        
        public string[][] Asks { get; set; }
    }
}