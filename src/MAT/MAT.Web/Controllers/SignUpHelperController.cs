using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Castle.Core.Logging;
using MAT.Core.InputModels;
using MAT.Core.Models;
using MAT.Core.Models.Enumerations;
using MAT.Core.Models.SignUp;
using MAT.Core.Models.Subscription;
using MAT.Core.Services;
using Newtonsoft.Json.Linq;
using Raven.Client;

namespace MAT.Web.Controllers
{
    public class SignUpHelperController : ApiController
    {
        private readonly IDocumentSession _ravenSession;
        private readonly ShippingCalculator _shippingCalculator;
        public ILogger Logger { get; set; }

        public SignUpHelperController(IDocumentSession documentSession, ShippingCalculator shippingCalculator)
        {
            _ravenSession = documentSession;
            _shippingCalculator = shippingCalculator;
        }
        
        [HttpPost]
        public dynamic ValidateReferralCode(JToken json)
        {
            if (string.IsNullOrEmpty(json["referralCode"].ToString()))
            {
                return new
                {
                    status = "Error",
                    message = "You must enter a promotion Code"
                };
            }

            try
            {
                Promotion promotion = _ravenSession.Query<Promotion>().FirstOrDefault(p => p.Code == json["referralCode"].ToString());
                if (promotion == null || !promotion.IsValid())
                {
                    return new
                    {
                        status = "Error",
                        message = "Promotion code is invalid"
                    };
                }

                return new
                    {
                        status = "Success",
                        message = promotion.ToString(),
                        discount = promotion.Amount
                    };
            }
            catch (Exception ex)
            {
                return new
                {
                    status = "Error",
                    message = ex.Message
                };
            }
        }

        [HttpPost]
        public dynamic LookupShippingRate(JToken json)
        {
            try
            {
                if (string.IsNullOrEmpty(json["postalCode"].ToString()))
                {
                    return new
                    {
                        status = "Error",
                        message = "You must enter a valid shipping information"
                    };
                }

                ShippingAreas area;
                if (!Enum.TryParse(json["area"].ToString(), true, out area))
                {
                    return new
                    {
                        status = "Error",
                        message = "You must enter a valid shipping information"
                    };
                }

                string shippingInfo = (area == ShippingAreas.FSA)
                                          ? json["postalCode"].ToString().Substring(0, 3)
                                          : json["province"].ToString();

                var rate = _shippingCalculator.FindRate(area, shippingInfo);
                return new
                {
                    status = "Success",
                    message = rate
                };
            }
            catch (Exception ex)
            {
                Logger.Error("Error with LookupShippingRate", ex);
                return new
                {
                    status = "Error",
                    message = "Shipping is not available to your Postal Code"
                };
            }
        }

        [HttpPost]
        public dynamic CalculateSignUpOrder(SignUpModel model)
        {
            try
            {
                List<SubscriptionProduct> bookBoxes = _ravenSession.Query<SubscriptionProduct>().Where(p => p.Status == ProductStatus.Active).ToList();

                var subscriptions =
                    model.Lines.Select(
                        li =>
                        (li.Subscription == "plus")
                            ? bookBoxes.Single(bb => bb.Name == "Book Box Plus")
                            : bookBoxes.Single(bb => bb.Name == "Book Box")).ToList();

                Province province = Enumeration.FromValueOrDefault(typeof(Province), model.Account.Province) as Province;

                var taxCalc = new TaxCalculator();
                var shippingPrice = _shippingCalculator.FindRate(model.Area, subscriptions, province, model.Account.FSA);

                var totalPrice = subscriptions.Sum(s => s.Price);
                var productTax = taxCalc.CalculateProductTax(totalPrice, model.Discount);
                var totalTax = productTax + shippingPrice.Tax;

                return new
                {
                    status = "Success",
                    tax = totalTax,
                    shipping = shippingPrice.Price
                };
            }
            catch (Exception ex)
            {
                Logger.Error("Error with CalculateOrderDetails", ex);

                return new
                {
                    status = "Error",
                    message = "Error calculating order"
                };
            }
        }

        [HttpPost]
        public dynamic CalculateGiftOrder(GiftPurchaseLineItem model)
        {
            try
            {
                List<SubscriptionProduct> bookBoxes = _ravenSession.Query<SubscriptionProduct>().Where(p => p.Status == ProductStatus.Active).ToList();
                
                var subscription =
                    model.Subscription == "plus"
                        ? bookBoxes.Single(bb => bb.Name == "Book Box Plus")
                        : bookBoxes.Single(bb => bb.Name == "Book Box");

                var giftLength = model.GiftLength;

                string shippingInfo = model.Recipient.Province;
                Province province = Enumeration.FromValue<Province>(Convert.ToInt32(shippingInfo));

                var taxCalc = new TaxCalculator();
                var shippingPrice = _shippingCalculator.FindRate(model.Area, new List<SubscriptionProduct> { subscription }, province, province.Abbreviation);

                var totalPrice = subscription.Price * giftLength;
                var productTax = taxCalc.CalculateProductTax(totalPrice);
                var totalTax = productTax + (shippingPrice.Tax * giftLength);

                return new
                {
                    status = "Success",
                    tax = totalTax,
                    shipping = shippingPrice.Price * giftLength
                };
            }
            catch (Exception ex)
            {
                Logger.Error("Error with CalculateOrderDetails", ex);

                return new
                {
                    status = "Error",
                    message = "Error calculating order"
                };
            }
        }
    }
}