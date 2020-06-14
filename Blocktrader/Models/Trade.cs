using System;
using System.Collections.Generic;
using System.Linq;

namespace Nexus.Blocktrader.Models
{
    public class Trade
    {
        public int Id { get; }
        public float Price { get; }
        public float Amount { get; }
        public bool IsSale { get; }
        public bool IsBuy => !IsSale;
        public DateTime Time { get; }

        public Trade(int id, float price, float amount, bool isSale, DateTime time)
        {
            Id = id;
            Price = price;
            Amount = amount;
            IsSale = isSale;
            Time = time;
        }

        public byte[] ToBytes()
        {
            var unixTimestamp = (long)(Time.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            unixTimestamp = unixTimestamp * 1000;

            return BitConverter.GetBytes(unixTimestamp)
                .Concat(BitConverter.GetBytes(Id))
                .Concat(BitConverter.GetBytes(Price))
                .Concat(BitConverter.GetBytes(Amount))
                .Concat(BitConverter.GetBytes(IsSale))
                .ToArray();
        }

        public static IEnumerable<Trade> FromBytes(byte[] bytes)
        {
            var index = 0;
            while (index < bytes.Length)
            {
                var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddMilliseconds(BitConverter.ToInt64(bytes, index));
                index += 8;
                var id = BitConverter.ToInt32(bytes, index);
                index += 4;
                var price = BitConverter.ToSingle(bytes, index);
                index += 4;
                var amount = BitConverter.ToSingle(bytes, index);
                index += 4;
                var isSale = BitConverter.ToBoolean(bytes, index);
                index += 1;

                yield return new Trade(id, price, amount, isSale, dateTime);
            }
        }
    }
}