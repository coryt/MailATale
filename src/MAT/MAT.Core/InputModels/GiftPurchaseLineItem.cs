using MAT.Core.Models;

namespace MAT.Core.InputModels
{
    public class GiftPurchaseLineItem
    {
        public GiftPaymentRecipientModel Recipient { get; set; }
        public string Subscription { get; set; }
        public int GiftLength { get; set; }
        public ShippingAreas Area { get; set; }
    }
}