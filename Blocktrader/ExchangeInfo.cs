using System.Collections.Generic;

namespace Blocktrader
{
    public class ExchangeInfo
    {
        public IEnumerable<Order> Bids { get; set; }
        
        public IEnumerable<Order> Asks { get; set; }
    }
}