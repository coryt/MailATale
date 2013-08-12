using System.Web.Mvc;

namespace MAT.Core.Models.Enumerations
{
    public class AddressType : Enumeration
    {
        public static readonly AddressType Shipping = new AddressType(0, "Shipping");
        public static readonly AddressType Billing = new AddressType(1, "Billing");

        private AddressType() { }
        private AddressType(int value, string displayName) : base(value, displayName) { }

        public static SelectList GetAddressTypeList()
        {
            return new SelectList(new[] { Shipping, Billing }, "Value", "DisplayName", null);
        }
    }
}