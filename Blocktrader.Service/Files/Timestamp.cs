using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nexus.Blocktrader.Domain;

namespace Nexus.Blocktrader.Service.Files
{
    public class Timestamp
    {
        public DateTime Date { get; }

        public TickerInfo TickerInfo { get; }
        
        public Timestamp(DateTime date, [NotNull] TickerInfo tickerInfo)
        {
            Date = date;
            TickerInfo = tickerInfo;
        }

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(Date.ToBinary())
                .Concat(BitConverter.GetBytes(TickerInfo.AveragePrice))
                .Concat(BitConverter.GetBytes(TickerInfo.OrderBook.Bids.Length))
                .Concat(TickerInfo.OrderBook.Bids.SelectMany(b => b.ToBytes()))
                .Concat(BitConverter.GetBytes(TickerInfo.OrderBook.Asks.Length))
                .Concat(TickerInfo.OrderBook.Asks.SelectMany(a => a.ToBytes()))
                .ToArray();
        }
        
        public byte[] ToBytes2()
        {
            var unixTimestamp = (long)(Date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            unixTimestamp = unixTimestamp * 1000;
            
            return BitConverter.GetBytes(unixTimestamp)
                .Concat(BitConverter.GetBytes(TickerInfo.AveragePrice))
                .Concat(BitConverter.GetBytes(TickerInfo.OrderBook.Bids.Length))
                .Concat(TickerInfo.OrderBook.Bids.SelectMany(b => b.ToBytes()))
                .Concat(BitConverter.GetBytes(TickerInfo.OrderBook.Asks.Length))
                .Concat(TickerInfo.OrderBook.Asks.SelectMany(a => a.ToBytes()))
                .ToArray();
        }

        public static IEnumerable<Timestamp> FromBytes(byte[] bytes)
        {
            var index = 0;
            while (index < bytes.Length)
            {
                var dateTime = DateTime.FromBinary(BitConverter.ToInt64(bytes, index));
                index += 8;
                var averagePrice = BitConverter.ToSingle(bytes, index);
                index += 4;
                var bidsCount = BitConverter.ToInt32(bytes, index);
                index += 4;
                var bids = new List<Order>(bidsCount);
                for (var i = 0; i < bidsCount; i++)
                {
                    bids.Add(Order.FromBytes(bytes, index));
                    index += 8;
                }

                var asksCount = BitConverter.ToInt32(bytes, index);
                index += 4;
                var asks = new List<Order>(bidsCount);
                for (var i = 0; i < asksCount; i++)
                {
                    asks.Add(Order.FromBytes(bytes, index));
                    index += 8;
                }

                yield return new Timestamp(dateTime, new TickerInfo(averagePrice, new OrderBook(bids.ToArray(), asks.ToArray()), dateTime));
            }
        }
    }
}