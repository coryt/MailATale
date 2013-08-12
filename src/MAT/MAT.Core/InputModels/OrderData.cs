using System;
using System.Collections.Generic;

namespace MAT.Core.InputModels
{
    public class OrderData
    {
        public string province { get; set; }
        public string postalCode { get; set; }
        public List<OrderLineItem> lineItems { get; set; }
        public decimal discount { get; set; }
        public string referralCode { get; set; }
    }
}