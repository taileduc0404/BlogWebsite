using System.ComponentModel.DataAnnotations;

namespace BlogWebsite.ViewModels
{
	public class EditProfileVM
	{
		public string? Id { get; set; }
		[Required]
		public string? Username { get; set; }
		[Required]
		public string? FirstName { get; set; }
		[Required]
		public string? LastName { get; set; }
		[Required]
		public string? Email { get; set; }
	}
}
