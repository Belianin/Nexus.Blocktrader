using System.Globalization;
using Nexus.Blocktrader.Models;

namespace Nexus.Blocktrader.Exchanges.Bitfinex.Models
{
    internal class OrderWithCount : Order
    {
        public int Count { get; }
        
        public static explicit operator OrderWithCount(string[] input)
        {
            return new OrderWithCount(
                float.Parse(input[0], CultureInfo.InvariantCulture),
                float.Parse(input[2], CultureInfo.InvariantCulture),
                int.Parse(input[1], CultureInfo.InvariantCulture));
        }

        public OrderWithCount(float price, float amount, int count) : base(price, amount)
        {
            Count = count;
        }
    }
}