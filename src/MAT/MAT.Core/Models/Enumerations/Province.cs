using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MAT.Core.Models.Enumerations
{
    public class Province : Enumeration
    {
        public static readonly Province ON = new Province(0, "Ontario", "ON");
        public static readonly Province QC = new Province(1, "Quebec", "QC");
        public static readonly Province NS = new Province(3, "Nova Scotia", "NS");
        public static readonly Province NB = new Province(4, "New Brunswick", "NB");
        public static readonly Province MB = new Province(5, "Manitoba", "MB");
        public static readonly Province BC = new Province(6, "British Columbia", "BC");
        public static readonly Province PE = new Province(7, "Prince Edward Island", "PE");
        public static readonly Province SK = new Province(8, "Saskatchewan", "SK");
        public static readonly Province AB = new Province(9, "Alberta", "AB");
        public static readonly Province NL = new Province(10, "Newfoundland and Labrador", "NL");
        public static readonly Province NT = new Province(11, "Northwest Territories", "NT");
        public static readonly Province YT = new Province(12, "Yukon", "YT");
        public static readonly Province NU = new Province(13, "Nunavut", "NU");

        private static readonly List<Province> Provinces = new List<Province>()
                { 
                    ON,
                    QC,
                    NS,
                    NB,
                    MB,
                    BC,
                    PE,
                    SK,
                    AB,
                    NL,
                    NT,
                    YT,
                    NU
                };
        private Province() { }
        private Province(int value, string displayName, string abbr)
            : base(value, displayName)
        {
            Abbreviation = abbr;
        }

        public string Abbreviation { get; set; }
        public static SelectList GetProvinces()
        {
            return new SelectList(Provinces, "Value", "DisplayName", null);
        }

        public static Province FindByAbbreviation(string abbreviation)
        {
            return abbreviation.Length == 2
                       ? Provinces.Single(p => p.Abbreviation == abbreviation)
                       : Provinces.Single(p => p.DisplayName == abbreviation);
        }
    }
}