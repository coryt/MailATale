using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace MAT.Core.InputModels
{
    public class LogOnModel
    {

        [Required]
        [Email]
        [Display(Name = "Email")]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
        public string DisplayMessage { get; set; }
    }
}