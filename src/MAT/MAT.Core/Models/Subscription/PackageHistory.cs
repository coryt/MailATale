using System;
using System.Collections.Generic;

namespace MAT.Core.Models.Subscription
{
    public class Package
    {
        public List<Book> Books { get; set; }
        public DateTime ShippedOn { get; set; }
        public string TrackingNumber { get; set; }
        public string ShippingCompany { get; set; }
        public DateTime DeliveredOn { get; set; }
    }
}