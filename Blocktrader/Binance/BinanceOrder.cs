using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Blocktrader.Binance
{
    public class BinanceOrder
    {
        public decimal Size { get; set; }
        
        public decimal Price { get; set; }

        public static explicit operator BinanceOrder(string[] input)
        {
            return new BinanceOrder
            {
                Price = decimal.Parse(input[0], CultureInfo.InvariantCulture),
                Size = decimal.Parse(input[1], CultureInfo.InvariantCulture)
            };
        }
    }
}