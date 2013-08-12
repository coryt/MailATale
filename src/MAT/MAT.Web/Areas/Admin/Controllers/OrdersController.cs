using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MAT.Core.Extensions;
using MAT.Core.Models;
using MAT.Core.Models.Gift;
using MAT.Core.Models.SignUp;
using MAT.Core.Models.Subscription;
using MAT.Web.Areas.Admin.Models;
using MAT.Web.Controllers;
using MAT.Web.Helpers.Filters;
using Raven.Client;
using Raven.Client.Linq;

namespace MAT.Web.Areas.Admin.Controllers
{
    [Admin]
    public class OrdersController : MATController
    {
        [HttpGet]
        public ActionResult Index(GridFilterSortOptions options)
        {
            var model = PagedGridFilterModel<OrderSummary>.From(options);

            var query = RavenSession.Query<OrderSummary>("OrderSummaries");

            switch (options.FilterEnum<OrderFilter>())
            {
                case OrderFilter.Gifts:
                    query = query.Where(o => o.OrderType == OrderType.Gift);
                    break;
                case OrderFilter.Subscriptions:
                    query = query.Where(o => o.OrderType == OrderType.Subscription);
                    break;
            }

            model.Items = query.OrderBy(model).Paginate(model.Page);
            ViewBag.Filters = EnumExtensions.ToSelectList<OrderFilter>();
            return View(model);            
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
            var order = RavenSession.Load<Order>(id);
            if (order == null)
                return RedirectToAction("Index");

            var model = new OrderDetailsViewModel(order);

            switch (model.OrderType)
            {
                case OrderType.Subscription:
                    var ids = ((order as SubscriptionOrder).SubscriptionIds ?? new List<string>())
                              .Where(x => x != null)
                              .Distinct();
                    if (ids.Any())
                        model.Subscriptions = RavenSession.Load<UserSubscription>(ids).ToList();
                    break;
                case OrderType.Gift:
                    var giftOrder = order as GiftOrder;
                    if (giftOrder.Purchase != null && !string.IsNullOrWhiteSpace(giftOrder.Purchase.Id))
                        model.GiftPurchase = RavenSession.Load<GiftPurchase>(giftOrder.Purchase.Id);
                    break;
            }

            return View(model);
        }
    }
}