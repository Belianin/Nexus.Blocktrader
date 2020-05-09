using System;
using System.Threading.Tasks;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Utils;

namespace Nexus.Blocktrader.Service.Files
{
    public interface ITimestampManager
    {
        Task WriteAsync(CommonTimestamp commonTimestamp);

        OldMonthTimestamp ReadTimestampsFromMonth(DateTime dateTime, Ticker ticker);
        Result<Timestamp[]> ReadTimestampForDay(DateTime dateTime, ExchangeTitle exchange, Ticker ticker);
    }
}