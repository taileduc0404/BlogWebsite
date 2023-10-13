using System.ComponentModel.DataAnnotations;

namespace BlogWebsite.Models
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
