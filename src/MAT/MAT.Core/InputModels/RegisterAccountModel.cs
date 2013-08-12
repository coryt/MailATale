using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAnnotationsExtensions;
using MAT.Core.Models;
using MAT.Core.Models.Enumerations;

namespace MAT.Core.InputModels
{
    public class RegisterAccountModel
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "E-mail address longer than 50 characters isn't valid.")]
        [Email]
        [Display(Name = "Email address (used as your login ID, too)")]
        public string Email { get; set; }

        [StringLength(50, ErrorMessage = "E-mail address longer than 50 characters isn't valid.")]
        [Email]
        [Display(Name = "Email address (used as your login ID, too)")]
        public string ConfirmEmail { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [Required]
        [Display(Name = "Street Address")]
        public string Street1 { get; set; }

        [Display(Name = "Unit/Apt")]
        public string Street2 { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Province")]
        public int Province { get; set; }

        [Required]
        [RegularExpression(@"^[ABCEGHJKLMNPRSTVXY|abceghjklmnprstvxy]{1}\d{1}[A-Z|a-z]{1} *\d{1}[A-Z|a-z]{1}\d{1}$")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required]
        [RegularExpression(@"[2-9](\d{6}|\d{9})$")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        public bool EmailMatches
        {
            get { return Email == ConfirmEmail; }
        }

        public string FSA
        {
            get
            {
                if (string.IsNullOrEmpty(PostalCode)) return string.Empty;
                return PostalCode.Substring(0, 3);
            }
        }
    }
}