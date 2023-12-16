using System.ComponentModel.DataAnnotations;

namespace BlogWebsite.ViewModels
{
	public class ResetPasswordVM
	{
		[Required]
		[DataType(DataType.Password)]
		public string? Password { get; set; }

		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string? ConfirmPassword { get; set; }

		public string? Email { get; set; }
		public string? Token { get; set; }
	}
}
