using System.Web.Mvc;
using MAT.Core.Models;
using MAT.Core.Models.Site;
using Raven.Client;

namespace MAT.Web
{
    public sealed class CheckForMaintenanceMode : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var docStore = MvcApplication.Container.Resolve<IDocumentStore>();
            using (var session = docStore.OpenSession())
            {
                var config = session.Load<SiteConfig>("Site/Config");
                if (bool.Parse(config.CurrentEnvironmentConfigs["MaintenanceMode"]))
                {
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.Redirect("maintenance.htm");
                    return;
                }
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}