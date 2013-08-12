using System;
using System.Collections.Generic;
using System.Linq;
using MAT.Core.InputModels;
using MAT.Core.Models.Enumerations;

namespace MAT.Core.Services
{
    public class TaxCalculator
    {
        private readonly Dictionary<Province, decimal> _taxSchedule = new Dictionary<Province, decimal>()
            {
                {Province.AB,new decimal(0.05)},
                {Province.BC,new decimal(0.12)},
                {Province.MB,new decimal(0.05)},
                {Province.NB,new decimal(0.13)},
                {Province.NL,new decimal(0.13)},
                {Province.NT,new decimal(0.05)},
                {Province.NU,new decimal(0.05)},
                {Province.ON,new decimal(0.13)},
                {Province.PE,new decimal(0.05)},
                {Province.QC,new decimal(0.05)},
                {Province.SK,new decimal(0.05)},
                {Province.YT,new decimal(0.05)},
                {Province.NS,new decimal(0.15)}
            };

        private readonly decimal _flatTaxRate = new decimal(0.05);

        public decimal CalculateProductTax(decimal totalPrice)
        {
            return CalculateProductTax(totalPrice, 0);
        }

        public decimal CalculateProductTax(decimal totalPrice, decimal discount)
        {
            var subTotal = totalPrice - discount;
            return Math.Round(subTotal * _flatTaxRate, 2);
        }

        public decimal CalculateShippingTax(decimal shippingFee, Province province)
        {
            return Math.Round(shippingFee * _taxSchedule[province], 2);
        }

        public decimal CalculateShippingTax(decimal shippingFee, string province)
        {
            var provinceEnum = Enumeration.FromDisplayName<Province>(province);
            return CalculateShippingTax(shippingFee, provinceEnum);
        }
    }
}