using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcContrib.Pagination;

namespace MAT.Web.Areas.Admin.Models
{
    public class PagedGridFilterModel<TItem> : GridFilterSortOptions
    {
        public IPagination<TItem> Items { get; set; }

        public static PagedGridFilterModel<TItem> From(GridFilterSortOptions options, string column = null)
        {
            var model = new PagedGridFilterModel<TItem>();
            if (options != null)
            {
                model.Page = options.Page;
                model.Filter = options.Filter;
                model.Column = options.Column;
                model.Direction = options.Direction;
            }

            if (string.IsNullOrWhiteSpace(model.Column))
                model.Column = column;

            return model;
        }
    }
}