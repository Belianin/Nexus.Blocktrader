using System;
using System.Collections.Generic;
using Blocktrader.Domain;

namespace Blocktrader.Service
{
    [Obsolete("Отстой какой-то")]
    public class Timestamp
    {
        public DateTime DateTime { get; set; }
        public Dictionary<string, ExchangeTimestamp> Exchanges { get; set; } = new Dictionary<string, ExchangeTimestamp>();
    }
}