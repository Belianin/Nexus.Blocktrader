using System;
using System.Threading.Tasks;
using Nexus.Blocktrader.Domain;

namespace Nexus.Blocktrader.Service.Files
{
    public interface ITimestampManager
    {
        Task WriteAsync(CommonTimestamp commonTimestamp);

        MonthTimestamp ReadTimestampsFromMonth(DateTime dateTime, Ticker ticker);
    }
}