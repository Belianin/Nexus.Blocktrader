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
        private readonly IReadOnlyCollection<Ticket> tickets = new[]
        {
            Ticket.BtcUsd,
            Ticket.EthBtc,
            Ticket.EthUsd,
            Ticket.XrpBtc,
            Ticket.XrpUsd,
        };
        
        public async Task WriteAsync(CommonTimestamp commonTimestamp)
        {
            foreach (var (ticket, info) in commonTimestamp.Binance.Tickers)
            {
                await using var writer = GetWriter(DateTime.Now, ExchangeTitle.Binance, ticket);
                var data = new Timestamp(commonTimestamp.DateTime, info);
                await writer.WriteAsync(data.ToBytes());
            }
            
            foreach (var (ticket, info) in commonTimestamp.Bitfinex.Tickers)
            {
                await using var writer = GetWriter(DateTime.Now, ExchangeTitle.Bitfinex, ticket);
                var data = new Timestamp(commonTimestamp.DateTime, info);
                await writer.WriteAsync(data.ToBytes());
            }
            
            foreach (var (ticket, info) in commonTimestamp.Bitstamp.Tickers)
            {
                await using var writer = GetWriter(DateTime.Now, ExchangeTitle.Bitstamp, ticket);
                var data = new Timestamp(commonTimestamp.DateTime, info);
                await writer.WriteAsync(data.ToBytes());
            }
        }

        public MonthTimestamp ReadTimestampsFromMonth(DateTime dateTime, Ticket ticket)
        {
            var result = new MonthTimestamp(dateTime, ticket);
            var binanceInfo = ReadTicketInfo(dateTime, ExchangeTitle.Binance, ticket);
            foreach (var (tick, info) in binanceInfo)
            {
                if (!result.Info.ContainsKey(tick))
                    result.Info[tick] = new Dictionary<ExchangeTitle, TicketInfo>();
                result.Info[tick][ExchangeTitle.Binance] = info;
            }

            var bitfinexInfo = ReadTicketInfo(dateTime, ExchangeTitle.Bitfinex, ticket);
            foreach (var (tick, info) in bitfinexInfo)
            {
                result.Info[tick][ExchangeTitle.Bitfinex] = info;
            }
            
            var bitstampInfo = ReadTicketInfo(dateTime, ExchangeTitle.Bitfinex, ticket);
            foreach (var (tick, info) in bitstampInfo)
            {
                result.Info[tick][ExchangeTitle.Bitstamp] = info;
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