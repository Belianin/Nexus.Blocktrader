using System.Globalization;

namespace Blocktrader.Bitfinex
{
    public class BitfinexOrder
    {
        public decimal Price { get; set; }
        
        public int Count { get; set; }
        
        public decimal Amount { get; set; }

        public static explicit operator BitfinexOrder(string[] input)
        {
            return new BitfinexOrder
            {
                Price = decimal.Parse(input[0], CultureInfo.InvariantCulture),
                Count = int.Parse(input[1], CultureInfo.InvariantCulture),
                Amount = decimal.Parse(input[2], CultureInfo.InvariantCulture)
            };
        }
    }
}