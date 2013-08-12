using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MAT.Core.Models;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;

namespace MAT.Core.InputModels
{
    public class ChangeAccountModel
    {
        public ChangeAccountModel()
        {
            
        }

        public ChangeAccountModel(User user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;

            if (user.Addresses.Count == 0)
            {
                ShippingAddress = new AddressModel(AddressType.Shipping.DisplayName);
                BillingAddress = new AddressModel(AddressType.Billing.DisplayName);
            }
            else
            {
                var shippingAddress = user.Addresses.Find(a => a.AddressType == AddressType.Shipping.DisplayName);
                var billingAddress = user.Addresses.Find(a => a.AddressType == AddressType.Billing.DisplayName);

                ShippingAddress = shippingAddress == null ? new AddressModel(AddressType.Shipping.DisplayName) : new AddressModel(shippingAddress);
                BillingAddress = billingAddress == null ? new AddressModel(AddressType.Billing.DisplayName) : new AddressModel(billingAddress);
            }
        }

        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required]
        [Display(Name = "First Name")] 
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public AddressModel ShippingAddress { get; set; }
        public AddressModel BillingAddress { get; set; }

        public bool UseShippingAsBilling { get; set; }
    }
}