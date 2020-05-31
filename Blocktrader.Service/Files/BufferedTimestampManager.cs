using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Utils;

namespace Nexus.Blocktrader.Service.Files
{
    public class BufferedTimestampManager : ITimestampManager
    {
        private readonly ITimestampManager innerManager;
        private readonly Dictionary<TimestampKey, MonthTimestamp> buffer;

        public BufferedTimestampManager(ITimestampManager innerManager)
        {
            buffer = new Dictionary<TimestampKey, MonthTimestamp>();
            this.innerManager = innerManager;
        }

        public Task WriteAsync(CommonTimestamp commonTimestamp)
        {
            return innerManager.WriteAsync(commonTimestamp);
        }

        public OldMonthTimestamp ReadTimestampsFromMonthOld(DateTime dateTime, Ticker ticker)
        {
            return innerManager.ReadTimestampsFromMonthOld(dateTime, ticker);
        }

        public Result<Timestamp[]> ReadTimestampForDay(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            var timestamp = ReadInnerTimestampForMonth(dateTime, exchange, ticker);
            if (timestamp.IsFail)
                return timestamp.Error;

            return timestamp.Value.GetForDay(dateTime.Day);
        }

        public Result<Timestamp[]> ReadTimestampForMonth(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            var timestamp = ReadInnerTimestampForMonth(dateTime, exchange, ticker);
            if (timestamp.IsFail)
                return timestamp.Error;

            return timestamp.Value.Timestamps;
        }

        private Result<MonthTimestamp> ReadInnerTimestampForMonth(DateTime dateTime, ExchangeTitle exchange, Ticker ticker)
        {
            var key = new TimestampKey{Year = dateTime.Year, Month = dateTime.Month, Exchange = exchange, Ticker = ticker};
            if (dateTime.Date != DateTime.Today && buffer.TryGetValue(key, out var buffered))
                return buffered;
            
            var timestamp = innerManager.ReadTimestampForMonth(dateTime, exchange, ticker);
            if (timestamp.IsFail)
                return "No timestamp";
            
            var monthTimestamp = new MonthTimestamp(dateTime, exchange, ticker, timestamp.Value);
            buffer[key] = monthTimestamp;
            return monthTimestamp;
        }

        private class TimestampKey
        {
            public int Year;
            public int Month;
            public ExchangeTitle Exchange;
            public Ticker Ticker;

            public override bool Equals(object obj)
            {
                if (!(obj is TimestampKey other))
                    return false;

                return other.Year == Year &&
                       other.Month == Month &&
                       other.Exchange == Exchange &&
                       other.Ticker == Ticker;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (int) Math.Pow(Year, Month) + Exchange.GetHashCode() + Ticker.GetHashCode(); // плохой хэш
                }
            }

            public static implicit operator TimestampKey(MonthTimestamp timestamp)
            {
                return new TimestampKey
                {
                    Year = timestamp.Year,
                    Month = timestamp.Month,
                    Ticker = timestamp.Ticker,
                    Exchange = timestamp.Exchange
                };
            }
        }
    }
}