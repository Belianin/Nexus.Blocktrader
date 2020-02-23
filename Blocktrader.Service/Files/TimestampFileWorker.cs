using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blocktrader.Domain;

namespace Blocktrader.Service.Files
{
    public class TimestampFileManager : ITimestampManager
    {
        public async Task WriteAsync(CommonTimestamp commonTimestamp)
        {
            foreach (var exchange in (ExchangeTitle[]) Enum.GetValues(typeof(ExchangeTitle)))
            {
                foreach (var (ticket, info) in commonTimestamp.Exchanges[exchange].Tickets)
                {
                    await using var writer = GetWriter(DateTime.Now, exchange, ticket);
                    var data = new Timestamp(commonTimestamp.DateTime, info);
                    await writer.WriteAsync(data.ToBytes());
                }
            }
        }

        public MonthTimestamp ReadTimestampsFromMonth(DateTime dateTime, Ticket ticket)
        {
            var result = new MonthTimestamp(dateTime, ticket);
            foreach (var exchange in (ExchangeTitle[]) Enum.GetValues(typeof(ExchangeTitle)))
            {
                var ticketInfo = ReadTicketInfo(dateTime, exchange, ticket);
                foreach (var (tick, info) in ticketInfo)
                {
                    if (!result.Info.ContainsKey(tick))
                        result.Info[tick] = new Dictionary<ExchangeTitle, TicketInfo>();
                    result.Info[tick][exchange] = info;
                }
            }

            return result;
        }

        private static Dictionary<DateTime, TicketInfo> ReadTicketInfo(DateTime dateTime, ExchangeTitle exchange, Ticket ticket)
        {
            var rawData = File.ReadAllBytes(GetFilename(dateTime, exchange, ticket));
            return Timestamp.FromBytes(rawData).ToDictionary(k => k.Date, v => v.TicketInfo);
        }

        private static string GetFilename(DateTime dateTime, ExchangeTitle exchange, Ticket ticket)
        {
            if (!Directory.Exists($"Data/{exchange.ToString()}"))
                Directory.CreateDirectory($"Data/{exchange.ToString()}");
            
            return $"Data/{exchange.ToString()}/{exchange.ToString()}_{ticket}_{dateTime.ToString("MMM_yyyy", new CultureInfo("en_US"))}";

        }

        private static FileStream GetWriter(DateTime dateTime, ExchangeTitle exchange, Ticket ticket)
        {
            return File.Open(GetFilename(dateTime, exchange, ticket), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        }
    }
}