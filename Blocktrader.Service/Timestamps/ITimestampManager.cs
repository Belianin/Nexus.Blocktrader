using System;
using System.Threading.Tasks;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Utils;

namespace Nexus.Blocktrader.Service.Timestamps
{
    public interface ITimestampManager
    {
        Task WriteAsync(CommonTimestamp commonTimestamp);
        Result<Timestamp[]> ReadTimestampForDay(DateTime dateTime, ExchangeTitle exchange, Ticker ticker);
        Result<Timestamp[]> ReadTimestampForMonth(DateTime dateTime, ExchangeTitle exchange, Ticker ticker);
        OldMonthTimestamp ReadTimestampsFromMonthOld(DateTime dateTime, Ticker ticker);
    }
}