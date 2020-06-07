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
        Task<Result<Trade[]>> ReadForDayAsync(ExchangeTitle exchange, Ticker ticker, DateTime day);
    }
}