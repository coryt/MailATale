using System;
using System.Collections.Generic;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;

namespace MAT.Web.Areas.Admin.Models
{
    public class UserSummary
    {
        public string Id { get; set; }
        public string FriendlyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool HasMonthly { get; set; }
        public bool HasBiMonthly { get; set; }
        public int SubscriptionCount { get; set; }

        public static UserSummary For(User user)
        {
            if (user == null) return null;
            return new UserSummary
            {
                Id = user.Id,
                FriendlyId = user.FriendlyId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }
    }
}