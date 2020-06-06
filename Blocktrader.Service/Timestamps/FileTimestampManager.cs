using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Utils;
using Nexus.Logging;

namespace Nexus.Blocktrader.Service.Timestamps
{
    public class FileTimestampManager : ITimestampManager
    {
        private readonly ILog log;
        private readonly string path;

        public FileTimestampManager(ILog log)
        {
            this.log = log;

            path = Directory.GetCurrentDirectory() + "\\Data";
            
            this.log.Important($"Current directory for data storage is \"{path}\"");
        }

        public async Task WriteAsync(CommonTimestamp commonTimestamp)
        {
            log.Debug($"Writing common timestamp for {commonTimestamp.DateTime:yyyy-MM}");
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

        public Result<Timestamp[]> ReadTimestampForMonth(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            var filename = GetFilename(dateTime, exchange, ticker);
            log.Debug($"Reading timestamp for month from \"{filename}\"");
            if (!File.Exists(filename))
            {
                log.Debug($"No such file \"{filename}\"");
                return "No such file";
            }

            var file = File.ReadAllBytes(filename);

            log.Debug($"Read a file with length {file.Length}");
            return Timestamp.FromBytes(file).ToArray();
        }

        public Result<Timestamp[]> ReadTimestampForDay(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            var readResult = ReadTimestampForMonth(dateTime, exchange, ticker);
            if (readResult.IsFail)
                return readResult;
            
            return readResult.Value.Where(t => t.Date.Day == dateTime.Day).ToArray();

        }

        private string GetFilename(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            if (!Directory.Exists($"{path}\\{exchange.ToString()}"))
            {
                log.Debug($"Creating folder for exchange {exchange.ToString()}");
                Directory.CreateDirectory($"{path}\\{exchange.ToString()}");
            }
            
            return $"{path}\\{exchange.ToString()}\\{exchange.ToString()}_{ticker}_{dateTime:MM_yyyy}";

        }

        private FileStream GetWriter(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            var filename = GetFilename(dateTime, exchange, ticker);
            
            log.Debug($"Writing to file \"{filename}\"");
            
            return File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        }

        public OldMonthTimestamp ReadTimestampsFromMonthOld(DateTime dateTime, Ticker ticker)
        {
            log.Debug($"Reading timestamps for {dateTime:yyyy-MM}");
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

        private Dictionary<DateTime, TickerInfo> ReadTicketInfo(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            var filename = GetFilename(dateTime, exchange, ticker);
            if (!File.Exists(filename))
                return new Dictionary<DateTime, TickerInfo>();
            
            var rawData = File.ReadAllBytes(filename);
            return Timestamp.FromBytes(rawData).ToDictionary(k => k.Date, v => v.TickerInfo);
        }
    }
}