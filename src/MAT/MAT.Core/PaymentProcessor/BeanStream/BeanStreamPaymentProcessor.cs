using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using MAT.Core.Extensions;
using MAT.Core.InputModels;
using MAT.Core.Models;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;
using MAT.Core.Models.Gift;
using MAT.Core.Models.SignUp;
using MAT.Core.Models.Site;
using MAT.Proxy.PaymentProcessor.BeanStream;

namespace MAT.Core.PaymentProcessor.BeanStream
{
    public class BeanStreamPaymentProcessor : IPaymentProcessor
    {
        private readonly ILogger _log;
        private readonly BeanStreamTransactionProxy _transactionProxy;
        private readonly BeanStreamRecurringProxy _recurringProxy;

        public BeanStreamPaymentProcessor(ILogger log, SiteConfig config)
        {
            _log = log;
            _transactionProxy = new BeanStreamTransactionProxy(log, config.CurrentEnvironmentConfigs["PaymentProcessing.BeanStream.MerchantId"], config.CurrentEnvironmentConfigs["PaymentProcessing.BeanStream.Username"], config.CurrentEnvironmentConfigs["PaymentProcessing.BeanStream.Password"]);
            _recurringProxy = new BeanStreamRecurringProxy(log, config.CurrentEnvironmentConfigs["PaymentProcessing.BeanStream.MerchantId"], config.CurrentEnvironmentConfigs["PaymentProcessing.BeanStream.Username"], config.CurrentEnvironmentConfigs["PaymentProcessing.BeanStream.Password"], config.CurrentEnvironmentConfigs["PaymentProcessing.BeanStream.Passcode"]);
        }

        public PaymentResponse ProcessPayment(SubscriptionOrder subscriptionOrder)
        {
            _log.Info(string.Format("Attempting to Process Transaction"));
            var request = _transactionProxy.GetDefaultRequest();
            request.Add(TransactionRequestFieldNames.trnOrderNumber, subscriptionOrder.Id.Replace("/", "_"));
            request.Add(TransactionRequestFieldNames.trnComments, String.Empty);
            request.Add(TransactionRequestFieldNames.trnCardOwner, subscriptionOrder.CreditCardName);
            request.Add(TransactionRequestFieldNames.trnCardNumber, subscriptionOrder.CreditCardNumber);
            request.Add(TransactionRequestFieldNames.trnCardCvd, subscriptionOrder.CreditCardSecurityCode);
            request.Add(TransactionRequestFieldNames.trnExpMonth, subscriptionOrder.CreditCardExpiryMonth);
            request.Add(TransactionRequestFieldNames.trnExpYear, subscriptionOrder.CreditCardExpiryYear.Substring(2, 2));
            request.Add(TransactionRequestFieldNames.ordName, subscriptionOrder.FullName);
            request.Add(TransactionRequestFieldNames.ordEmailAddress, subscriptionOrder.Email);
            request.Add(TransactionRequestFieldNames.ordPhoneNumber, subscriptionOrder.BillingAddress.Phone);
            request.Add(TransactionRequestFieldNames.ordAddress1, subscriptionOrder.BillingAddress.Street1);
            request.Add(TransactionRequestFieldNames.ordAddress2, subscriptionOrder.BillingAddress.Street2);
            request.Add(TransactionRequestFieldNames.ordCity, subscriptionOrder.BillingAddress.City);
            request.Add(TransactionRequestFieldNames.ordProvince, Enumeration.FromDisplayName<Province>(subscriptionOrder.BillingAddress.Province).Abbreviation);
            request.Add(TransactionRequestFieldNames.ordPostalCode, subscriptionOrder.BillingAddress.PostalCode);
            request.Add(TransactionRequestFieldNames.ordCountry, ParseCountryISOCode(subscriptionOrder.BillingAddress.Country));
            request.Add(TransactionRequestFieldNames.shipName, subscriptionOrder.FullName);
            request.Add(TransactionRequestFieldNames.shipEmailAddress, subscriptionOrder.Email);
            request.Add(TransactionRequestFieldNames.shipPhoneNumber, subscriptionOrder.ShippingAddress.Phone);
            request.Add(TransactionRequestFieldNames.shipAddress1, subscriptionOrder.ShippingAddress.Street1);
            request.Add(TransactionRequestFieldNames.shipAddress2, subscriptionOrder.ShippingAddress.Street2);
            request.Add(TransactionRequestFieldNames.shipCity, subscriptionOrder.ShippingAddress.City);
            request.Add(TransactionRequestFieldNames.shipProvince, Enumeration.FromDisplayName<Province>(subscriptionOrder.ShippingAddress.Province).Abbreviation);
            request.Add(TransactionRequestFieldNames.shipPostalCode, subscriptionOrder.ShippingAddress.PostalCode);
            request.Add(TransactionRequestFieldNames.shipCountry, ParseCountryISOCode(subscriptionOrder.ShippingAddress.Country));
            request.Add(TransactionRequestFieldNames.ordItemPrice, subscriptionOrder.SubTotal.ToString());
            request.Add(TransactionRequestFieldNames.ordShippingPrice, subscriptionOrder.ShippingPrice.Price.ToString());
            request.Add(TransactionRequestFieldNames.ordTax1Price, subscriptionOrder.OrderTax.ToString());
            request.Add(TransactionRequestFieldNames.ordTax2Price, subscriptionOrder.ShippingPrice.Tax.ToString());
            request.Add(TransactionRequestFieldNames.trnAmount, subscriptionOrder.Total.ToString());
            request.Add(TransactionRequestFieldNames.trnRecurring, "1");
            request.Add(TransactionRequestFieldNames.rbBillingPeriod, "M");
            request.Add(TransactionRequestFieldNames.rbFirstBilling,
                      subscriptionOrder.DateCreated.Day > 10 // cutoff date for the month
                          ? subscriptionOrder.DateCreated.NextMonth()
                          : subscriptionOrder.DateCreated.CurrentMonth());
            request.Add(TransactionRequestFieldNames.rbBillingIncrement, "1");

            var transactionResponse = _transactionProxy.ProcessRequest(request);
            transactionResponse.VerifyResponse(_log);

            switch (transactionResponse.Response)
            {
                case ResponseStates.Approved:
                    return PaymentResponse.Success(transactionResponse);

                case ResponseStates.Denied:
                    return PaymentResponse.Denied(transactionResponse);

                case ResponseStates.InvalidInput:
                    return PaymentResponse.InvalidInput(transactionResponse);

                case ResponseStates.ExternalError:
                    return PaymentResponse.ExternalError(transactionResponse);

                default:
                    return PaymentResponse.Error(transactionResponse);
            }
        }

        public PaymentResponse ProcessPayment(GiftOrder order)
        {
            _log.Info(string.Format("Attempting to Process Transaction"));
            var request = _transactionProxy.GetDefaultRequest();
            request.Add(TransactionRequestFieldNames.trnOrderNumber, order.Id.Replace("/", "_"));
            request.Add(TransactionRequestFieldNames.trnComments, string.Format("Gift purchase for ({0}) GPId {1}", order.Purchase.RecipientEmail, order.Purchase.Id));
            request.Add(TransactionRequestFieldNames.trnCardOwner, order.CreditCardName);
            request.Add(TransactionRequestFieldNames.trnCardNumber, order.CreditCardNumber);
            request.Add(TransactionRequestFieldNames.trnCardCvd, order.CreditCardSecurityCode);
            request.Add(TransactionRequestFieldNames.trnExpMonth, order.CreditCardExpiryMonth);
            request.Add(TransactionRequestFieldNames.trnExpYear, order.CreditCardExpiryYear.Substring(2, 2));
            request.Add(TransactionRequestFieldNames.ordName, order.Purchase.SenderName);
            request.Add(TransactionRequestFieldNames.ordEmailAddress, order.Purchase.SenderEmail);
            request.Add(TransactionRequestFieldNames.ordPhoneNumber, order.BillingAddress.Phone);
            request.Add(TransactionRequestFieldNames.ordAddress1, order.BillingAddress.Street1);
            request.Add(TransactionRequestFieldNames.ordAddress2, order.BillingAddress.Street2);
            request.Add(TransactionRequestFieldNames.ordCity, order.BillingAddress.City);
            request.Add(TransactionRequestFieldNames.ordProvince, Enumeration.FromDisplayName<Province>(order.BillingAddress.Province).Abbreviation);
            request.Add(TransactionRequestFieldNames.ordPostalCode, order.BillingAddress.PostalCode);
            request.Add(TransactionRequestFieldNames.ordCountry, ParseCountryISOCode(order.BillingAddress.Country));
            request.Add(TransactionRequestFieldNames.shipName, order.Purchase.SenderName);
            request.Add(TransactionRequestFieldNames.shipEmailAddress, order.Purchase.SenderEmail);
            request.Add(TransactionRequestFieldNames.shipPhoneNumber, order.ShippingAddress.Phone);
            request.Add(TransactionRequestFieldNames.shipAddress1, order.ShippingAddress.Street1);
            request.Add(TransactionRequestFieldNames.shipAddress2, order.ShippingAddress.Street2);
            request.Add(TransactionRequestFieldNames.shipCity, order.ShippingAddress.City);
            request.Add(TransactionRequestFieldNames.shipProvince, Enumeration.FromDisplayName<Province>(order.ShippingAddress.Province).Abbreviation);
            request.Add(TransactionRequestFieldNames.shipPostalCode, order.ShippingAddress.PostalCode);
            request.Add(TransactionRequestFieldNames.shipCountry, ParseCountryISOCode(order.ShippingAddress.Country));
            request.Add(TransactionRequestFieldNames.ordItemPrice, order.SubTotal.ToString());
            request.Add(TransactionRequestFieldNames.ordShippingPrice, order.ShippingPrice.Price.ToString());
            request.Add(TransactionRequestFieldNames.ordTax1Price, order.OrderTax.ToString());
            request.Add(TransactionRequestFieldNames.ordTax2Price, order.ShippingPrice.Tax.ToString());
            request.Add(TransactionRequestFieldNames.trnAmount, order.Total.ToString());
            request.Add(TransactionRequestFieldNames.trnRecurring, "0");

            var transactionResponse = _transactionProxy.ProcessRequest(request);
            transactionResponse.VerifyResponse(_log);

            switch (transactionResponse.Response)
            {
                case ResponseStates.Approved:
                    return PaymentResponse.Success(transactionResponse);

                case ResponseStates.Denied:
                    return PaymentResponse.Denied(transactionResponse);

                case ResponseStates.InvalidInput:
                    return PaymentResponse.InvalidInput(transactionResponse);

                case ResponseStates.ExternalError:
                    return PaymentResponse.ExternalError(transactionResponse);

                default:
                    return PaymentResponse.Error(transactionResponse);
            }
        }

        public PaymentResponse ModifyAccount(User userInfo, Address billingAddress, Address shippingAddress, PaymentInfoModel paymentInfoModel)
        {
            if (billingAddress == null && shippingAddress == null)
                return PaymentResponse.InvalidInput("Address information must be provided");

            if (billingAddress == null)
                billingAddress = shippingAddress;
            if (shippingAddress == null)
                shippingAddress = billingAddress;

            if (userInfo.PaymentProviderId == null)
            {
                _log.Warn(string.Format("Attempting to Modify Account for user ({0}) Email: {1} but they do not have a payment provider id.", userInfo.Id, userInfo.Email));
                return PaymentResponse.Error("Tehre was a problem updating your account");
            }

            return ModifyAccount(userInfo.PaymentProviderId, userInfo, billingAddress, shippingAddress, paymentInfoModel);
        }

        public PaymentResponse ModifyAccount(string userPaymentProviderId, User userInfo, Address billingAddress, Address shippingAddress, PaymentInfoModel paymentInfoModel)
        {
            _log.Info(string.Format("Attempting to Modify Account"));
            var request = _recurringProxy.GetDefaultRequest();
            if (paymentInfoModel != null)
            {
                request.Add(RecurringRequestFieldNames.trnCardOwner, paymentInfoModel.CreditCardName);
                request.Add(RecurringRequestFieldNames.trnCardNumber, paymentInfoModel.CreditCardNumber);
                request.Add(RecurringRequestFieldNames.trnExpMonth, paymentInfoModel.CreditCardExpiryMonth);
                request.Add(RecurringRequestFieldNames.trnExpYear, paymentInfoModel.CreditCardExpiryYear.Substring(2, 2));
            }

            request.Add(RecurringRequestFieldNames.ordName, userInfo.FirstName + " " + userInfo.LastName);
            request.Add(RecurringRequestFieldNames.ordEmailAddress, userInfo.Email);
            request.Add(RecurringRequestFieldNames.ordPhoneNumber, billingAddress.Phone);
            request.Add(RecurringRequestFieldNames.ordAddress1, billingAddress.Street1);
            request.Add(RecurringRequestFieldNames.ordAddress2, billingAddress.Street2);
            request.Add(RecurringRequestFieldNames.ordCity, billingAddress.City);
            request.Add(RecurringRequestFieldNames.ordProvince, Enumeration.FromDisplayName<Province>(billingAddress.Province).Abbreviation);
            request.Add(RecurringRequestFieldNames.ordPostalCode, billingAddress.PostalCode);
            request.Add(RecurringRequestFieldNames.ordCountry, ParseCountryISOCode(billingAddress.Country));
            request.Add(RecurringRequestFieldNames.shipName, userInfo.FirstName + " " + userInfo.LastName);
            request.Add(RecurringRequestFieldNames.shipEmailAddress, userInfo.Email);
            request.Add(RecurringRequestFieldNames.shipPhoneNumber, shippingAddress.Phone);
            request.Add(RecurringRequestFieldNames.shipAddress1, shippingAddress.Street1);
            request.Add(RecurringRequestFieldNames.shipAddress2, shippingAddress.Street2);
            request.Add(RecurringRequestFieldNames.shipCity, shippingAddress.City);
            request.Add(RecurringRequestFieldNames.shipProvince, Enumeration.FromDisplayName<Province>(shippingAddress.Province).Abbreviation);
            request.Add(RecurringRequestFieldNames.shipPostalCode, ParseCountryISOCode(shippingAddress.PostalCode));
            request.Add(RecurringRequestFieldNames.shipCountry, shippingAddress.Country);

            request.Add(RecurringRequestFieldNames.rbAccountId, userPaymentProviderId);
            request.Add(RecurringRequestFieldNames.operationType, "M");

            var response = _recurringProxy.ProcessRequest(request);
            response.VerifyResponse(_log);

            switch (response.Response)
            {
                case ResponseStates.Approved:
                    _log.Info("Modify Account call was successful");
                    return PaymentResponse.Success();

                case ResponseStates.InvalidInput:
                    _log.Error(string.Format("Modify Account call failed. {0}. {1}", response.Message));
                    return PaymentResponse.Error(response.Message);

                default:
                    _log.Error(string.Format("Modify Account call failed. {0}. {1}", response.Message));
                    return PaymentResponse.Error(response.Message);
            }
        }

        private string ParseCountryISOCode(string country)
        {
            var isoCodes = new Dictionary<string, string> { { "Canada", "CA" } };

            return isoCodes.ContainsKey(country) ? isoCodes[country] : country;
        }
    }
}