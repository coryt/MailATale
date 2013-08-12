using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MAT.Core.Extensions;
using MAT.Core.Models;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;
using MAT.Core.Models.Subscription;
using MAT.Core.Services;
using MAT.Web.Areas.Admin.Models;
using MAT.Web.Controllers;
using MAT.Web.Helpers.Filters;
using MvcContrib.UI.Grid;
using Raven.Client;
using Raven.Client.Linq;

namespace MAT.Web.Areas.Admin.Controllers
{
    [Admin]
    public class UsersController : MATController
    {
        [HttpGet]
        public ActionResult Index(GridFilterSortOptions options)
        {
            var model = PagedGridFilterModel<UserSummary>.From(options, "Email");

            var query = RavenSession.Query<UserSummary>("UserSummaries");

            switch (options.FilterEnum<UserFilter>())
            {
                case UserFilter.ActiveMonthlySubscriptions:
                    query = query.Where(us => us.HasMonthly);
                    break;
                case UserFilter.ActiveBiMonthlySubscriptions:
                    query = query.Where(us => us.HasBiMonthly);
                    break;
            }

            model.Items = query.OrderBy(model).Paginate(model.Page);
            ViewBag.Filters = EnumExtensions.ToSelectList<UserFilter>();
            return View(model);
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
            var user = RavenSession.Load<User>(id);
            if (user == null)
                return RedirectToAction("Index");

            var subscriptions = RavenSession.Query<UserSubscription>()
                                .Where(us => us.UserId == user.Id);

            return View(new UserDetailsViewModel(user, subscriptions.ToList()));
        }
    }
}