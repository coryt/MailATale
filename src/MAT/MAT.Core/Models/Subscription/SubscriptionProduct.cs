
using MAT.Core.Models.Enumerations;

namespace MAT.Core.Models.Subscription
{
    public class SubscriptionProduct
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public ProductStatus Status { get; set; }

        public string FriendlyId { get { return Id.Substring(Id.IndexOf("/") + 1); } }
    }
}