using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Utils;

namespace Nexus.Blocktrader.Service.Trades
{
    public interface ITradesManager
    {
        Task WriteAsync(ExchangeTitle exchange, IEnumerable<Trade> trades, Ticker ticker);
        Task<Result<Trade[]>> ReadAsync(ExchangeTitle exchange, Ticker ticker, DateTime from, TimeSpan period);
    }
}