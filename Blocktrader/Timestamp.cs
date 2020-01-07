using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocktrader
{
    public class Timestamp
    {
        public DateTime Date { get; set; }
        
        public Order[] Orders { get; set; }

        public byte[] ToBytes()
        {
            var result = BitConverter.GetBytes(Date.ToBinary())
                .Concat(BitConverter.GetBytes(Orders.Length));

            foreach (var order in Orders)
            {
                result = result.Concat(order.ToBytes());
            }

            return result.ToArray();
        }

        public static IEnumerable<Timestamp> FromBytes(byte[] bytes)
        {
            var index = 0;
            while (true)
            {
                
                var dateTime = DateTime.FromBinary(BitConverter.ToInt64(bytes, index));
                index += 8;
                var length = BitConverter.ToInt32(bytes, index);
                index += 4;
                var orders = new List<Order>(length);
                for (var i = 0; i < length; i++)
                {
                    orders.Add(Order.FromBytes(bytes, index));
                    index += 8;
                }

                yield return new Timestamp
                {
                    Date = dateTime,
                    Orders = orders.ToArray()
                };
            }
        }
    }
}