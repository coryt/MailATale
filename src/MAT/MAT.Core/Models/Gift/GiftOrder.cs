using System;
using System.Collections.Generic;
using MAT.Core.InputModels;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;
using MAT.Core.Models.SignUp;
using MAT.Core.Models.Subscription;
using MAT.Core.PaymentProcessor;
using MAT.Core.Services;

namespace MAT.Core.Models.Gift
{
    public class GiftOrder : Order
    {
        private readonly ShippingCalculator _shippingCalculator;
        private readonly TaxCalculator _taxCalculator;

        public GiftPurchase Purchase { get; set; }

        public GiftOrder() { }

        public GiftOrder(GiftPurchase purchase, Address billingAddress, PaymentInfoModel paymentInfoModel, ShippingCalculator shippingCalculator)
        {
            Purchase = purchase;
            _taxCalculator = new TaxCalculator();
            _shippingCalculator = shippingCalculator;

            Discount = new decimal(0);
            CreditCardName = paymentInfoModel.CreditCardName;
            CreditCardNumber = paymentInfoModel.CreditCardNumber;
            CreditCardSecurityCode = paymentInfoModel.CreditCardSecurityCode;
            CreditCardExpiryMonth = paymentInfoModel.CreditCardExpiryMonth;
            CreditCardExpiryYear = paymentInfoModel.CreditCardExpiryYear;

            BillingAddress = billingAddress;
            ShippingAddress = billingAddress;
            OrderStatus = OrderStatuses.PaymentPending;
            DateCreated = DateTime.Now;
            UpdateOrderTotals();
        }

        private void UpdateOrderTotals()
        {
            var province = Enumeration.FromDisplayName<Province>(Purchase.RecipientProvince);
            var baseShipping = _shippingCalculator.FindRate(ShippingAreas.Province,
                                                         new List<SubscriptionProduct> { Purchase.SubscriptionProduct },
                                                         province,
                                                         province.Abbreviation);

            ShippingPrice = new ShippingPrice(baseShipping.Price*Purchase.SubscriptionLength, baseShipping.Tax*Purchase.SubscriptionLength);
            SubTotal = Purchase.SubscriptionProduct.Price * Purchase.SubscriptionLength;
            CalculateOrderTax();
            Total = Math.Round(SubTotal + ShippingPrice.Price + OrderTax + ShippingPrice.Tax, 2);
        }

        protected override void CalculateOrderTax()
        {
            OrderTax = _taxCalculator.CalculateProductTax(SubTotal);
        }

        public override void ProcessPayment(IPaymentProcessor paymentProcessor)
        {
            base.ProcessPayment(paymentProcessor);

            if (TransactionResponse != PaymentResults.Approved) return;
            OrderStatus = OrderStatuses.Paid;
        }
    }
}