using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocktrader
{
    public class Timestamp
    {
        public DateTime Date { get; set; }
        
        public Order[] Bids { get; set; }
        
        public Order[] Asks { get; set; }

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(Date.ToBinary())
                .Concat(BitConverter.GetBytes(Bids.Length))
                .Concat(Bids.SelectMany(b => b.ToBytes()))
                .Concat(BitConverter.GetBytes(Asks.Length))
                .Concat(Asks.SelectMany(a => a.ToBytes()))
                .ToArray();
        }

        public static IEnumerable<Timestamp> FromBytes(byte[] bytes)
        {
            var index = 0;
            while (index < bytes.Length)
            {
                Console.WriteLine(index);
                var dateTime = DateTime.FromBinary(BitConverter.ToInt64(bytes, index));
                index += 8;
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

                Console.WriteLine(dateTime);
                yield return new Timestamp
                {
                    Date = dateTime,
                    Bids = bids.ToArray(),
                    Asks = asks.ToArray()
                };
            }
        }
    }
}