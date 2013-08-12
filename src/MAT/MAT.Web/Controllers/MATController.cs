using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MAT.Core.Controllers;
using MAT.Core.Models;
using MAT.Core.Models.Site;
using MAT.Web.Helpers.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Raven.Json.Linq;

namespace MAT.Web.Controllers
{
    public abstract class MATController : RavenWebController
    {
        private SiteConfig _siteConfig;

        protected SiteConfig SiteConfig
        {
            get
            {
                if (_siteConfig == null)
                {
                    _siteConfig = RavenSession.Load<SiteConfig>("Site/Config");
                    if (_siteConfig == null)
                    {
                        throw new Exception("SiteConfig is not found.");
                    }
                }
                return _siteConfig;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            ViewBag.SiteConfig = SiteConfig;
        }

        protected new JsonNetResult Json(object data)
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return new JsonNetResult(data, settings);
        }

        protected string SerializeData(object data)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(data);
        }
    }
}