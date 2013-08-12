using System;

namespace MAT.Core.Models.SignUp
{
    public class Promotion
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string DiscountType { get; set; }
        public bool Active { get; set; }
        public DateTime? ValidThruStartDate { get; set; }
        public DateTime? ValidThruEndDate { get; set; }
        public int MigrationId { get; set; }

        public bool IsValid()
        {
            if (!Active)
                return false;
            var now = DateTime.Now;

            if (ValidThruStartDate == null && ValidThruEndDate == null)
                return true;

            return !(now < ValidThruStartDate || now > ValidThruEndDate);
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2} discount will be applied.", DiscountType == "Money" ? "$" : string.Empty, Amount,
                DiscountType == "Percent" ? "%" : string.Empty);
        }
    }
}