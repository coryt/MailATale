using System.Web.Mvc;
using MAT.Core.Controllers;
using MAT.Web.Infrastructure.Common;

namespace MAT.Web.Helpers.Filters
{
    public class AdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext == null) return;

            var controller = filterContext.Controller as RavenWebController;
            if (controller != null && controller.RavenSession != null)
            {
                var user = controller.RavenSession.GetCurrentUser();
                if (user != null && user.IsAdmin) return;
            }

            filterContext.Result = new HttpNotFoundResult();
        }
    }
}
