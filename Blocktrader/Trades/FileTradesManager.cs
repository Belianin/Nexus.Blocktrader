using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nexus.Blocktrader.Models;
using Nexus.Core;
using Nexus.Logging;

namespace Nexus.Blocktrader.Trades
{
    public class FileTradesManager : ITradesManager
    {
        private readonly ILog log;
        private readonly string path;
        
        public FileTradesManager(ILog log)
        {
            this.log = log;

            path = Directory.GetCurrentDirectory() + "\\Data";
        }
        
        public async Task WriteAsync(ExchangeTitle exchange, IEnumerable<Trade> trades, Ticker ticker)
        {
            var groups = trades.GroupBy(t => t.Time.Day);
            foreach (var @group in groups)
            {
                var groupedTrades = group.ToList();
                await using var writer = GetWriter(groupedTrades.First().Time, exchange, ticker);

                await writer.WriteAsync(groupedTrades.SelectMany(t => t.ToBytes()).ToArray())
                    .ConfigureAwait(false);
            }
        }

        [Obsolete("ToDo period")]
        public async Task<Result<Trade[]>> ReadForDayAsync(ExchangeTitle exchange, Ticker ticker, DateTime day)
        {
            var filename = GetFilename(day, exchange, ticker);
            log.Debug($"Reading trades for month from \"{filename}\"");
            if (!File.Exists(filename))
            {
                log.Debug($"No such file \"{filename}\"");
                return "No such file";
            }

            var file = await File.ReadAllBytesAsync(filename).ConfigureAwait(false);

            log.Debug($"Read a file with length {file.Length}");
            return Trade.FromBytes(file).Where(t => t.Time.Day == day.Day).ToArray();
        }
        
        private FileStream GetWriter(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            var filename = GetFilename(dateTime, exchange, ticker);
            
            log.Debug($"Writing to file \"{filename}\"");
            
            return File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        }
        
        private string GetFilename(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            if (!Directory.Exists($"{path}\\{exchange.ToString()}"))
            {
                log.Debug($"Creating folder for exchange {exchange.ToString()}");
                Directory.CreateDirectory($"{path}\\{exchange.ToString()}");
            }
            
            return $"{path}\\{exchange.ToString()}\\{exchange.ToString()}_Trades_{ticker}_{dateTime:MM_yyyy}";

        }
    }
}