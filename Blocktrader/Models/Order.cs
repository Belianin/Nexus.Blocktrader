using System;
using System.Collections.Generic;
using System.Linq;

namespace Nexus.Blocktrader.Models
{
    public class Order
    {
        public float Price { get; }

        public float Amount { get; }

        public Order(float price, float amount)
        {
            Price = price;
            Amount = amount;
        }

        public IEnumerable<byte> ToBytes()
        {
            return BitConverter.GetBytes(Convert.ToSingle(Price))
                .Concat(BitConverter.GetBytes(Convert.ToSingle(Amount)));
        }

        public static Order FromBytes(byte[] bytes, int index)
        {
            return new Order(
                BitConverter.ToSingle(bytes, index),
                BitConverter.ToSingle(bytes, index + 4));
        }
    }
}