using System;
using MAT.Core.Models;

namespace MAT.Web.Areas.Admin.Models
{
    public class OrderSummary
    {
        public string Id { get; set; }
        public OrderType OrderType { get; set; }
        public OrderDetails Order { get; set; }
    }

    public class OrderDetails : Order
    {
        protected override void CalculateOrderTax()
        {
            throw new NotImplementedException();
        }

        public override void ProcessPayment(Core.PaymentProcessor.IPaymentProcessor paymentProcessor)
        {
            throw new NotImplementedException();
        }
    }
}