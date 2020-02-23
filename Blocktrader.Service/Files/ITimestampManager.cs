using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blocktrader.Domain;

namespace Blocktrader.Service.Files
{
    public interface ITimestampManager
    {
        Task WriteAsync(CommonTimestamp commonTimestamp);

        Task<MonthTimestamp> ReadTimestampsFromMonthAsync(DateTime dateTime, Ticket ticket);
    }
}