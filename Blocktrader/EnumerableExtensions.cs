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
    }
}