using System;
using MAT.Core.Models.Account;
using MAT.Core.Models.SignUp;
using MAT.Core.PaymentProcessor;
using MAT.Proxy.PaymentProcessor.BeanStream;
using Raven.Imports.Newtonsoft.Json;

namespace MAT.Core.Models
{
    public abstract class Order
    {
        public ShippingPrice ShippingPrice;
        public string Id { get; set; }
        public DateTime DateCreated { get; set; }
        public OrderStatuses OrderStatus { get; set; }
        public decimal OrderTax { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }

        [JsonIgnore]
        public string CreditCardName { get; set; }

        [JsonIgnore]
        public string CreditCardNumber { get; set; }

        [JsonIgnore]
        public string CreditCardSecurityCode { get; set; }

        [JsonIgnore]
        public string CreditCardExpiryMonth { get; set; }

        [JsonIgnore]
        public string CreditCardExpiryYear { get; set; }

        public Address BillingAddress { get; set; }
        public Address ShippingAddress { get; set; }
        public string PaymentMessage { get; set; }
        public string PaymentResult { get; set; }
        public TransactionResponse TransactionResponse { get; set; }
        public decimal Discount { get; set; }
        protected abstract void CalculateOrderTax();
        public abstract void ProcessPayment(IPaymentProcessor paymentProcessor);
    }
}