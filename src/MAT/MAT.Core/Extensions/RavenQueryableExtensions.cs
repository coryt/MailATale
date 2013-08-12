using System;
using System.Linq.Expressions;
using MAT.Core.Models;
using MvcContrib.Pagination;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;
using Raven.Client.Linq;

namespace MAT.Core.Extensions
{
    public static class RavenQueryableExtensions
    {
        public static IPagination<T> Paginate<T>(this IRavenQueryable<T> query, int? pageNumber = PaginationDefaults.PageNumber, int? pageSize = PaginationDefaults.PageSize)
        {
            return new LazyRavenQueryablePagination<T>(query, pageNumber, pageSize);
        }

        public static IRavenQueryable<T> OrderBy<T>(this IRavenQueryable<T> query, GridSortOptions sort)
        {
            if (sort == null) return query;
            return query.OrderBy(sort.Column, sort.Direction);
        }

        public static IRavenQueryable<T> OrderBy<T>(this IRavenQueryable<T> query, string propertyName, SortDirection direction)
        {
            return SortExtensions.OrderBy(query, propertyName, direction) as IRavenQueryable<T>;
        }
    }
}
