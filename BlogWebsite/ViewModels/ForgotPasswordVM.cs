using System.ComponentModel.DataAnnotations;

namespace BlogWebsite.ViewModels
{
	public class ForgotPasswordVM
	{
		[Required]
		[EmailAddress]
		public string? Email { get; set; }
	}
}
