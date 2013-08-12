using System;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;

namespace MAT.Core.Models.Subscription
{
    public class UserSubscription
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime LastBilledOn { get; set; }
        public SubscriptionStatus SubscriptionStatus { get; set; }
        public DeliveryFrequency DeliveryFrequency { get; set; }
        public SubscriptionProduct SubscriptionProduct { get; set; }
        public Reader Reader { get; set; }
        public Address ShippingAddress { get; set; }
    }
}