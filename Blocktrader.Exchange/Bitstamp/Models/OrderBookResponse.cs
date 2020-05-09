using System;

namespace Nexus.Blocktrader.Exchange.Bitstamp.Models
{
    [Obsolete("Проверить на лишние поля")]
    internal class OrderBookResponse
    {
        public long LastUpdateId { get; set; }
        
        public int Timestamp { get; set; }
        
        public string[][] Bids { get; set; }
        
        public string[][] Asks { get; set; }
    }
}