using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Utils;

namespace Nexus.Blocktrader.Service.Files
{
    public class FileTimestampManager : ITimestampManager
    {
        private readonly ILogger log;

        public FileTimestampManager(ILogger log)
        {
            this.log = log;
        }

        public async Task WriteAsync(CommonTimestamp commonTimestamp)
        {
            log.LogDebug($"Writing common timestamp for {commonTimestamp.DateTime:yyyy-MM}");
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

        public OldMonthTimestamp ReadTimestampsFromMonth(DateTime dateTime, Ticker ticker)
        {
            log.LogDebug($"Reading timestamps for {dateTime:yyyy-MM}");
            var result = new OldMonthTimestamp(dateTime, ticker);
            foreach (var exchange in (ExchangeTitle[]) Enum.GetValues(typeof(ExchangeTitle)))
            {
                var ticketInfo = ReadTicketInfo(dateTime, exchange, ticker);
                foreach (var (tick, info) in ticketInfo)
                {
                    if (!result.Info.ContainsKey(tick))
                        result.Info[tick] = new Dictionary<ExchangeTitle, TickerInfo>();
                    result.Info[tick][exchange] = info;
                }
            }

            return result;
        }

        public Result<Timestamp[]> ReadTimestampForDay(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            var fileName = GetFilename(dateTime, exchange, ticker);
            if (!File.Exists(fileName))
                return "No such file";

            var file = File.ReadAllBytes(fileName);

            return Timestamp.FromBytes(file).ToArray();

        }

        private static Dictionary<DateTime, TickerInfo> ReadTicketInfo(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            var filename = GetFilename(dateTime, exchange, ticker);
            if (!File.Exists(filename))
                return new Dictionary<DateTime, TickerInfo>();
            
            var rawData = File.ReadAllBytes(filename);
            return Timestamp.FromBytes(rawData).ToDictionary(k => k.Date, v => v.TickerInfo);
        }

        private static string GetFilename(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            if (!Directory.Exists($"Data/{exchange.ToString()}"))
                Directory.CreateDirectory($"Data/{exchange.ToString()}");
            
            var path = $"Data/{exchange.ToString()}/{exchange.ToString()}_{ticker}_{dateTime:MM_yyyy}";
            Console.WriteLine(path);
            return path;

        }

        private static FileStream GetWriter(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            return File.Open(GetFilename(dateTime, exchange, ticker), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        }
    }
}