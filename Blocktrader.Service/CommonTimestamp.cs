using System;
using System.Collections.Generic;
using Blocktrader.Domain;

namespace Blocktrader.Service
{
    public class CommonTimestamp
    {
        public DateTime DateTime { get; set; }
        
        public Dictionary<ExchangeTitle, ExchangeTimestamp> Exchanges { get; set; }
    }
}