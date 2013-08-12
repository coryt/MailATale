using System.Web.Mvc;

namespace MAT.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CheckForMaintenanceMode());
        }
    }
}