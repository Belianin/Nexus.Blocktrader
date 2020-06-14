using System;
using System.Collections.Generic;
using Nexus.Blocktrader.Models;

namespace Nexus.Blocktrader
{
    public class CommonTimestamp
    {
        public DateTime DateTime { get; set; }
        
        public Dictionary<ExchangeTitle, ExchangeTimestamp> Exchanges { get; set; }
    }
}