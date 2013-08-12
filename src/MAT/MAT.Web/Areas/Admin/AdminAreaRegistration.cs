using System.Web.Mvc;

namespace MAT.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Admin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "admin/{controller}/{action}/{id}",
                new { controller = "Default", action = "Index", id = UrlParameter.Optional },
                new[] { "MAT.Web.Areas.Admin.Controllers" });

            context.MapRoute(
                "Admin_details",
                "admin/{controller}/Details/{*id}",
                new { controller = "Default", action = "Details", id = UrlParameter.Optional },
                new[] { "MAT.Web.Areas.Admin.Controllers" });
        }
    }
}