using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MAT.Core.Utilities
{
    public static class LinqExtensions
    {
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, SelectIndexProvider> collectionSelector,
            Func<TSource, int, TResult> resultSelector)
        {
            return source.Select(resultSelector);
        }

        public static SelectIndexProvider GetIndex<T>(this T element)
        {
            return null;
        }

        public static IQueryable<TResult> SelectMany<TSource, TResult>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, SelectIndexProvider>> collectionSelector,
            Expression<Func<TSource, int, TResult>> resultSelector)
        {
            return source.Select(resultSelector);
        }
    }
}