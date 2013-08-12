using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace MAT.Core.InputModels
{
    public class GiftPurchaseModel
    {
        public GiftPurchaseModel()
        {
            PaymentInfo = new PaymentInfoModel();
            BillingAddress = new AddressModel();
            Lines = new List<GiftPurchaseLineItem>() { new GiftPurchaseLineItem() };
        }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "E-mail address longer than 50 characters isn't valid.")]
        [Email]
        [Display(Name = "Email address (used as your login ID, too)")]
        public string Email { get; set; }

        public List<GiftPurchaseLineItem> Lines { get; set; }

        public PaymentInfoModel PaymentInfo { get; set; }

        public AddressModel BillingAddress { get; set; }

        public string PersonalMessage { get; set; }
        public string SendGiftNoticeOn { get; set; }
    }
}