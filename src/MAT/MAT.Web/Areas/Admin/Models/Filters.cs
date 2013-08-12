using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MAT.Web.Areas.Admin.Models
{
    public enum UserFilter
    {
        None,
        ActiveMonthlySubscriptions,
        ActiveBiMonthlySubscriptions
    }

    public enum SubscriptionFilter
    {
        None,
        ActiveMonthly,
        ActiveBiMonthly
    }

    public enum OrderFilter
    {
        None,
        Gifts,
        Subscriptions
    }
}