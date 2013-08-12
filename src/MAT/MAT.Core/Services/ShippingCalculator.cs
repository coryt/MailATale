using System;
using System.Collections.Generic;
using System.Linq;
using MAT.Core.Models;
using MAT.Core.Models.Enumerations;
using MAT.Core.Models.Subscription;
using Raven.Client;

namespace MAT.Core.Services
{
    public class ShippingCalculator
    {
        private readonly IDocumentSession _documentSession;
        private readonly TaxCalculator _taxCalculator;

        public ShippingCalculator(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
            _taxCalculator = new TaxCalculator();
        }

        public decimal FindRate(ShippingAreas area, string fsa)
        {
            var rate = _documentSession.Query<ShippingRate>().FirstOrDefault(sr => sr.Value == fsa && sr.Type == area);
            if (rate != null)
                return rate.Rate;

            throw new ArgumentException("Shipping Rate Not Found");
        }
        
        public ShippingPrice FindRate(ShippingAreas area, List<SubscriptionProduct> subscriptions, Province province, string fsa)
        {
            // if the customer is ordering multiple boxes, shipping = free
            // if the customer is ordering a double box, shipping = free
            // if the shipping rate is in an area that is > 6.95 shipping isn't free
            decimal rate = FindRate(area, fsa);
            decimal tax = _taxCalculator.CalculateShippingTax(rate, province);
            if (area == ShippingAreas.Province)
            {
                return subscriptions.Any(s => s.Price > (decimal) 19.95)
                           ? ShippingPrice.NoShippingFee()
                           : new ShippingPrice(rate, tax);
            }
            if(rate >= (decimal) 6.95) return new ShippingPrice(rate, tax);

            if (subscriptions.Count() >= 2)
                return ShippingPrice.NoShippingFee();

            if (subscriptions.Any(s=>s.Price > (decimal)19.95))
                return ShippingPrice.NoShippingFee();

            return new ShippingPrice(rate, tax);
        }
    }
}