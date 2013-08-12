using System.Web.Mvc;
using MAT.Web.Controllers;
using MAT.Web.Helpers.Filters;

namespace MAT.Web.Areas.Admin.Controllers
{
    [Admin]
    public class DefaultController : MATController
    {
        public ActionResult Index()
        {
            return View(base.SiteConfig);
        }
    }
}