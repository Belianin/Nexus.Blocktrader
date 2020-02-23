using System;

namespace Blocktrader.Service
{
    public class Timestamp
    {
        public DateTime DateTime { get; set; }
        public ExchangeTimestamp Binance { get; set; }
        public ExchangeTimestamp Bitfinex { get; set; }
        public ExchangeTimestamp Bitstamp { get; set; }
    }
}