using System.ComponentModel.DataAnnotations;

namespace Quilt4.Web.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string ErrorMessage { get; set; }
        public bool EmailEnabled { get; set; }
    }
}
