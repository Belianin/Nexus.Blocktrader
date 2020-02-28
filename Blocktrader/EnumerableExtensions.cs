using System;
using System.Collections.Generic;
using System.Linq;
using Blocktrader.Domain;

namespace Blocktrader
{
    public static class EnumerableExtensions
    {
        private static IEnumerable<Order> Flat(
            this IEnumerable<Order> orders,
            Func<Order, float> priceGetter,
            Func<Order, float, bool> predicate)
        {
            var currentAmount = 0f;
            var price = priceGetter(orders.First());
            foreach (var order in orders)
            {
                if (predicate(order, price))
                {
                    currentAmount += order.Amount;
                }
                else
                {
                    yield return new Order
                    {
                        Amount = currentAmount,
                        Price = price
                    };
                    price = priceGetter(order);
                    currentAmount = order.Amount;
                }
            }
        }
        public static IEnumerable<Order> Flat(this IEnumerable<Order> orders, int precision, bool isBid)
        {
            if (precision < 0) 
                return orders;
            
            var delta = (float) Math.Pow(10, precision);
            return isBid
                ? orders.Flat(o => (float) Math.Ceiling(o.Price / delta) * delta, (o, p) => p - o.Price >= 0)
                : orders.Flat(o => (float) Math.Floor(o.Price / delta) * delta, (o, p) => p - o.Price <= 0);
        }
    }
}