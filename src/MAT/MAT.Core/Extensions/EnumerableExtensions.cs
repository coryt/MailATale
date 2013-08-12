using System;
using System.Collections.Generic;

namespace MAT.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null || action == null) yield break;
            foreach (T item in source)
            {
                action(item);
                yield return item;
            }
        }
    }
}
