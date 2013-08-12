using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcContrib.UI.Grid;

namespace MAT.Web.Areas.Admin.Models
{
    public class GridFilterSortOptions : GridSortOptions
    {
        public int? Page { get; set; }
        public string Filter { get; set; }

        public T FilterEnum<T>() where T : struct
        {
            T result;
            return Enum.TryParse(Filter, true, out result) ? result : default(T);
        }
    }
}