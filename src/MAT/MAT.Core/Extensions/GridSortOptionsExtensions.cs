using System;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;

namespace MAT.Core.Extensions
{
    public static class GridSortOptionsExtensions
    {
        public static GridSortOptions Default(this GridSortOptions sort, string column = null, SortDirection direction = SortDirection.Ascending)
        {
            sort = sort ?? new GridSortOptions { Column = column, Direction = direction };
            if (string.IsNullOrWhiteSpace(sort.Column))
                sort.Column = column;
            return sort;
        }
    }
}
