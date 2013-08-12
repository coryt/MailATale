using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MvcContrib.Pagination;
using Raven.Client;
using Raven.Client.Linq;

namespace MAT.Core.Models
{
    /// <remarks>
    /// Adapted MvcContrib <see cref="LazyPagination<T>"/> for Raven.
    /// </remarks>
    public class LazyRavenQueryablePagination<T> : IPagination<T>
    {
        private RavenQueryStatistics _stats;
        private IList<T> _results;

        public LazyRavenQueryablePagination(IRavenQueryable<T> query, int? pageNumber = PaginationDefaults.PageNumber, int? pageSize = PaginationDefaults.PageSize)
        {
            pageNumber = pageNumber ?? PaginationDefaults.PageNumber;
            pageSize = pageSize ?? PaginationDefaults.PageSize;

            if (query == null)
                throw new ArgumentNullException("query");
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "Page number must be >= 1");
            if (pageSize < 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "Page size must be >= 0");

            PageNumber = pageNumber.Value;
            PageSize = pageSize.Value;
            Query = query;
        }

        /// <summary>
        /// Gets the query to execute.
        /// </summary>
        public IRavenQueryable<T> Query { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public int FirstItem
        {
            get
            {
                Execute();
                return ((PageNumber - 1) * PageSize) + 1;
            }
        }

        public int LastItem
        {
            get
            {
                Execute();
                return FirstItem + _results.Count - 1;
            }
        }

        public bool HasNextPage
        {
            get { return PageNumber < TotalPages; }
        }

        public bool HasPreviousPage
        {
            get { return PageNumber > 1; }
        }

        public int TotalItems
        {
            get
            {
                Execute();
                return _stats.TotalResults;
            }
        }

        public int TotalPages
        {
            get
            {
                Execute();
                return (int)Math.Ceiling((double)TotalItems / PageSize);
            }
        }

        /// <summary>
        /// Executes the query if it hasn't been executed yet.
        /// </summary>
        protected void Execute()
        {
            if (_results != null) return;            
            var skip = (PageNumber - 1) * PageSize;
            _results = Query.Statistics(out _stats).Skip(skip).Take(PageSize).ToList();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            Execute();
            foreach (var item in _results)
                yield return item;
        }
    }
}
