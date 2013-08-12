using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Xml.Linq;
using Castle.Core.Logging;
using MAT.Core.Extensions;
using Raven.Client;

namespace MAT.Core.Controllers
{
    public class RavenWebController : Controller
    {
        public IDocumentSession RavenSession { get; set; }
        public IDocumentStore DocumentStore { get; set; }
        public ILogger Logger { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = DocumentStore.OpenSession();
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            using (RavenSession)
            {
                if(Session != null && filterContext.Exception == null)
                {
                    RavenSession.SaveChanges();
                }
            }
            base.OnActionExecuted(filterContext);
        }

        protected HttpStatusCodeResult HttpNotModified()
        {
            return new HttpStatusCodeResult(304);
        }

        protected ActionResult Xml(XDocument xml, string etag)
        {
            return new XmlResult(xml, etag);
        }
    }
}
