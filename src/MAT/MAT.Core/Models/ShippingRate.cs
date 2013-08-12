namespace MAT.Core.Models
{
    public class ShippingRate
    {
        public string Value { get; set; }
        public ShippingAreas Type { get; set; }
        public decimal Rate { get; set; }
    }
}