using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MAT.Core.Extensions;

namespace MAT.Core.InputModels
{
    public class ForgotPasswordModel
    {
        public ForgotPasswordModel()
        {
            ShowResult = "hide";
        }

        public string Email { get; set; }
        public string ShowResult { get; set; }
        public string Result { get; set; }
        public string GenerateToken()
        {
            return Guid.NewGuid().ToString().EncodeTo64();
        }
    }

    public class ResetPasswordModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "New password is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm new password")]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
    }
}