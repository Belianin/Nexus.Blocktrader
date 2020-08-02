using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nexus.Blocktrader.Models;
using Nexus.Core;

namespace Nexus.Blocktrader.Trades
{
    public interface ITradesManager
    {
        Task WriteAsync(ExchangeTitle exchange, IEnumerable<Trade> trades, Ticker ticker);
        Task<Result<Trade[]>> ReadForDayAsync(ExchangeTitle exchange, Ticker ticker, DateTime day);
        Task<Result<Trade[]>> ReadForMonthAsync(ExchangeTitle exchange, Ticker ticker, DateTime month);
    }
}