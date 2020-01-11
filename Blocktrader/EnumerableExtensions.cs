using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocktrader
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<Order> Flat(this IEnumerable<Order> orders, int delta)
        {
            var currentAmount = 0f;
            var currentPrice = orders.First().Price;
            foreach (var order in orders.OrderByDescending(a => a.Price))
            {
                if (Math.Abs(currentPrice - order.Price) <= delta)
                {
                    currentAmount += order.Amount;
                }
                else
                {
                    yield return new Order
                    {
                        Amount = currentAmount,
                        Price = currentPrice
                    };
                    currentPrice = order.Price;
                    currentAmount = order.Amount;
                }
            }
            yield return new Order
            {
                Amount = currentAmount,
                Price = currentPrice
            };
        }
        
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
            {
                action(element);
                yield return element;
            }
        }
    }
}