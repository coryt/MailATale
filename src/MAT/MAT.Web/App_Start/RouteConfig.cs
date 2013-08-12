using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using MAT.Core.Routing;

namespace MAT.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
           
            routes.MapRouteLowerCase(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", area = "", id = UrlParameter.Optional },
                namespaces: new[] { "MAT.Web.Controllers" }
                );

            routes.MapRouteLowerCase("homepage",
                                     "",
                                     new {controller = "Home", action = "Index", area = ""},
                                     new[] {"MAT.Web.Controllers"}
                );
        }
    }
}