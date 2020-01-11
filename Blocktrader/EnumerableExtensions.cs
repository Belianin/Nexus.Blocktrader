using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocktrader
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<Order> Flat(this IEnumerable<Order> orders, int precision, bool bid)
        {
            if (precision >= 0)
            {
                var currentAmount = 0f;
                var delta = Convert.ToSingle(Math.Pow(10, precision));
                var price = Convert.ToSingle(Math.Ceiling(orders.First().Price / delta) * delta);
                if (bid) {price = Convert.ToSingle(Math.Floor(orders.First().Price / delta) * delta);}

                foreach (var order in orders)
                {
                    var bidask = (price - order.Price >= 0);
                    if (bid) {bidask = (price - order.Price <= 0);}

                    if (bidask)
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
                        price = Convert.ToSingle(Math.Ceiling(order.Price / delta) * delta);
                        if (bid) {price = Convert.ToSingle(Math.Floor(order.Price / delta) * delta);}
                            currentAmount = order.Amount;
                    }
                }
              
            }
            else
            {
                foreach (var order in orders) 
                {
                    yield return new Order
                    {
                        Amount = order.Amount,
                        Price = order.Price
                    };
                }
            }
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