using System;
using System.Collections.Generic;
using Nexus.Blocktrader.Domain;

namespace Nexus.Blocktrader.Service
{
    public class CommonTimestamp
    {
        public DateTime DateTime { get; set; }
        
        public Dictionary<ExchangeTitle, ExchangeTimestamp> Exchanges { get; set; }
    }
}