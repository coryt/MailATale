namespace MAT.Core.Models
{
    public class ShippingPrice
    {
        public decimal Price { get; private set; }
        public decimal Tax { get; private set; }

        public ShippingPrice(decimal price, decimal tax)
        {
            Price = price;
            Tax = tax;
        }

        public static ShippingPrice NoShippingFee()
        {
            return new ShippingPrice(0, 0);
        }
    }
}