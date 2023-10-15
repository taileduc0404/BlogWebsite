using System.ComponentModel.DataAnnotations;

namespace BlogWebsite.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string? Email { get; set; }
    }
}
