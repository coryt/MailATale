using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MAT.Core.Models;
using MAT.Core.Models.Gift;
using MAT.Core.Models.SignUp;
using MAT.Core.Models.Subscription;
using MAT.Web.Areas.Admin.Models;

namespace MAT.Web.Areas.Admin.Models
{
    public class OrderDetailsViewModel
    {
        private Order _order = null;
        private List<UserSubscription> _subscriptions = new List<UserSubscription>();

        public OrderDetailsViewModel() : this(null)
        {
        }

        public OrderDetailsViewModel(Order order)
        {
            Order = order;
        }

        public Order Order
        {
            get { return _order; }
            set
            {
                _order = value;
                OrderType = OrderType.Unknown;
                if (_order is SubscriptionOrder)
                    OrderType = OrderType.Subscription;
                else if (_order is GiftOrder)
                    OrderType = OrderType.Gift;
            }
        }

        public OrderType OrderType { get; private set; }

        // SubscriptionOrder
        public List<UserSubscription> Subscriptions
        {
            get { return _subscriptions; }
            set { _subscriptions = value ?? new List<UserSubscription>(); }
        }

        // GiftOrder
        public GiftPurchase GiftPurchase { get; set; }
    }
}