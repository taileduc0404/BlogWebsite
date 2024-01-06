using Microsoft.AspNetCore.Identity;

namespace BlogWebsite.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public bool IsLocked { get; set; }

	}
}
