using System.Globalization;

namespace Blocktrader.Bitfinex
{
    public class OrderWithCount : Order
    {
        public int Count { get; set; }
        
        public static explicit operator OrderWithCount(string[] input)
        {
            return new OrderWithCount
            {
                Price = float.Parse(input[0], CultureInfo.InvariantCulture),
                Count = int.Parse(input[1], CultureInfo.InvariantCulture),
                Amount = float.Parse(input[2], CultureInfo.InvariantCulture)
            };
        }
    }
}