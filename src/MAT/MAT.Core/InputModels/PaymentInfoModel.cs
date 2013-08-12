using System.ComponentModel.DataAnnotations;

namespace MAT.Core.InputModels
{
    public class PaymentInfoModel
    {
        [Required]
        [Display(Name = "Name on Card")]
        public string CreditCardName { get; set; }
        [Required]
        [Display(Name = "Card Number")]
        public string CreditCardNumber { get; set; }
        [Required]
        [Display(Name = "Expiry Month")]
        public string CreditCardExpiryMonth { get; set; }
        [Required]
        [Display(Name = "Expiry Year")]
        public string CreditCardExpiryYear { get; set; }
        [Required]
        [Display(Name = "Security Code")]
        public string CreditCardSecurityCode { get; set; }
    }
}