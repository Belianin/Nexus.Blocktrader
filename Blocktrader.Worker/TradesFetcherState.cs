using System.Collections.Generic;
using Nexus.Blocktrader.Domain;

namespace Blocktrader.Worker
{
    public class TradesFetcherState
    {
        public Dictionary<ExchangeTitle, Dictionary<Ticker, int>> LastIds { get; set; }
        public int MinimumAmount { get; set; }
    }
}