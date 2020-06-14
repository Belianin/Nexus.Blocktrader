using System.Collections.Generic;
using Nexus.Blocktrader.Models;

namespace Nexus.Blocktrader.Worker
{
    public class TradesFetcherState
    {
        public Dictionary<ExchangeTitle, Dictionary<Ticker, int>> LastIds { get; set; }
        public int MinimumAmount { get; set; }
    }
}