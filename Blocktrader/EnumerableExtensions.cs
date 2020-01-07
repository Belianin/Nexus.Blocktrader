using System;
using System.Collections.Generic;

namespace Blocktrader
{
    public static class EnumerableExtensions
    {
        public static void Update<T>(this ICollection<T> source, IEnumerable<T> update)
        {
            source.Clear();
            foreach (var u in update)
            {
                source.Add(u);
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