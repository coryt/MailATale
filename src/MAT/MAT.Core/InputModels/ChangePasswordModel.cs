using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MAT.Core.InputModels
{
    public class ChangePasswordModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Current password is required.")]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "New password is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm new password")]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}