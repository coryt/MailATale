using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MAT.Core.Extensions;
using MAT.Core.Models.Enumerations;
using MAT.Core.Models.Subscription;
using MAT.Web.Areas.Admin.Models;
using MAT.Web.Controllers;
using MAT.Web.Helpers.Filters;
using Raven.Client;
using Raven.Client.Linq;

namespace MAT.Web.Areas.Admin.Controllers
{
    [Admin]
    public class SubscriptionsController : MATController
    {
        [HttpGet]
        public ActionResult Index(GridFilterSortOptions options)
        {
            var model = PagedGridFilterModel<UserSubscription>.From(options);

            var query = RavenSession.Query<UserSubscription>();

            switch (options.FilterEnum<SubscriptionFilter>())
            {
                case SubscriptionFilter.ActiveMonthly:
                    query = query.Where(us => us.SubscriptionStatus == SubscriptionStatus.Active &&
                                              us.DeliveryFrequency == DeliveryFrequency.Monthly);
                    break;
                case SubscriptionFilter.ActiveBiMonthly:
                    query = query.Where(us => us.SubscriptionStatus == SubscriptionStatus.Active &&
                                              us.DeliveryFrequency == DeliveryFrequency.BiMonthly);
                    break;
            }

            model.Items = query.OrderBy(model).Paginate(model.Page);
            ViewBag.Filters = EnumExtensions.ToSelectList<SubscriptionFilter>();
            return View(model);
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
            var subscription = RavenSession.Load<UserSubscription>(id);
            if (subscription == null)
                return RedirectToAction("Index");
            return View(subscription);
        }
    }
}