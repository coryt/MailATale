using System;
using System.Collections.Generic;
using System.Linq;
using MAT.Core.InputModels;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;
using MAT.Core.Models.Subscription;
using MAT.Core.PaymentProcessor;
using MAT.Core.Services;
using Raven.Imports.Newtonsoft.Json;

namespace MAT.Core.Models.SignUp
{
    public class SubscriptionOrder : Order
    {
        private readonly ShippingCalculator _shippingCalculator;
        private readonly TaxCalculator _taxCalculator;

        public SubscriptionOrder() { }

        public SubscriptionOrder(User userInfo, Address billingAddress, Address shippingAddress, PaymentInfoModel paymentInfoModel, Promotion promo, List<UserSubscription> subscriptions, ShippingCalculator shippingCalculator)
        {
            _taxCalculator = new TaxCalculator();
            _shippingCalculator = shippingCalculator;

            UserId = userInfo.Id;
            Email = userInfo.Email;
            FullName = userInfo.FirstName + " " + userInfo.LastName;

            Discount = promo == null ? new decimal(0) : promo.Amount;

            CreditCardName = paymentInfoModel.CreditCardName;
            CreditCardNumber = paymentInfoModel.CreditCardNumber;
            CreditCardSecurityCode = paymentInfoModel.CreditCardSecurityCode;
            CreditCardExpiryMonth = paymentInfoModel.CreditCardExpiryMonth;
            CreditCardExpiryYear = paymentInfoModel.CreditCardExpiryYear;

            BillingAddress = billingAddress;
            ShippingAddress = shippingAddress;
            OrderStatus = OrderStatuses.PaymentPending;
            DateCreated = DateTime.Now;
            Subscriptions = subscriptions;
            SubscriptionIds = subscriptions.Select(us => us.Id).ToList();

            UpdateOrderTotals();
        }

        private void UpdateOrderTotals()
        {
            ShippingPrice = _shippingCalculator.FindRate(ShippingAreas.FSA,
                                                         Subscriptions.Select(us => us.SubscriptionProduct).ToList(),
                                                         Enumeration.FromDisplayName<Province>(ShippingAddress.Province),
                                                         ShippingAddress.FSA);
            CalculateOrderTax();

            Total = Math.Round(SubTotal + ShippingPrice.Price + OrderTax + ShippingPrice.Tax, 2);
        }

        //account properties
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }

        //package properties
        [JsonIgnore]
        public List<UserSubscription> Subscriptions { get; set; }
        public List<string> SubscriptionIds;

        protected override void CalculateOrderTax()
        {
            SubTotal = Subscriptions.Sum(s => s.SubscriptionProduct.Price) - Discount;
            OrderTax = _taxCalculator.CalculateProductTax(SubTotal);
        }

        public override void ProcessPayment(IPaymentProcessor paymentProcessor)
        {
            var response = paymentProcessor.ProcessPayment(this);
            TransactionResponse = response.Response;
            PaymentResult = response.Result.ToString();
            PaymentMessage = response.Message;

            if (response.Result != PaymentResults.Approved) return;

            OrderStatus = OrderStatuses.Paid;
            foreach (var subscription in Subscriptions)
            {
                DateTime orderDate;
                if (!DateTime.TryParse(response.Response.Date, out orderDate))
                    orderDate = DateTime.Now;

                subscription.LastBilledOn = orderDate;
                subscription.SubscriptionStatus = SubscriptionStatus.Active;
            }
        }
    }
}