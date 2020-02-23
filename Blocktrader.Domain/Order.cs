using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocktrader.Domain
{
    public class Order
    {
        public float Price { get; set; }
        
        public float Amount { get; set; }

        public IEnumerable<byte> ToBytes()
        {
            return BitConverter.GetBytes(Convert.ToSingle(Price))
                .Concat(BitConverter.GetBytes(Convert.ToSingle(Amount)));
        }

        public static Order FromBytes(byte[] bytes, int index)
        {
            return new Order
            {
                Price = BitConverter.ToSingle(bytes, index),
                Amount = BitConverter.ToSingle(bytes, index + 4),
            };
        }
    }

    public enum OrderType
    {
        Bid,
        Ask
    }
}