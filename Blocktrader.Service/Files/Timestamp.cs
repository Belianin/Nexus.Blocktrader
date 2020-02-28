using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Blocktrader.Domain;

namespace Blocktrader.Service.Files
{
    public class Timestamp
    {
        public DateTime Date { get; }

        public TicketInfo TicketInfo { get; }
        
        public Timestamp(DateTime date, [NotNull] TicketInfo ticketInfo)
        {
            Date = date;
            TicketInfo = ticketInfo;
        }

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(Date.ToBinary())
                .Concat(BitConverter.GetBytes(TicketInfo.AveragePrice))
                .Concat(BitConverter.GetBytes(TicketInfo.OrderBook.Bids.Length))
                .Concat(TicketInfo.OrderBook.Bids.SelectMany(b => b.ToBytes()))
                .Concat(BitConverter.GetBytes(TicketInfo.OrderBook.Asks.Length))
                .Concat(TicketInfo.OrderBook.Asks.SelectMany(a => a.ToBytes()))
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

                yield return new Timestamp(dateTime,
                    new TicketInfo(
                        averagePrice,
                        new OrderBook {Bids = bids.ToArray(), Asks = asks.ToArray()}));
            }
        }
    }
}