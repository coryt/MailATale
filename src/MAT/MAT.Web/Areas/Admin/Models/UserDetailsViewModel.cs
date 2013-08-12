using System;
using System.Collections.Generic;
using MAT.Core.Models.Account;
using MAT.Core.Models.Subscription;

namespace MAT.Web.Areas.Admin.Models
{
    public class UserDetailsViewModel
    {
        public UserDetailsViewModel() { }
        public UserDetailsViewModel(User user, List<UserSubscription> subscriptions)
        {
            User = user;
            Subscriptions = subscriptions;
        }

        public User User { get; set; }
        public List<UserSubscription> Subscriptions { get; set; }
    }
}