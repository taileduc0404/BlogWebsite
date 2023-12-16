using System.ComponentModel.DataAnnotations;

namespace BlogWebsite.ViewModels
{
	public class CreateTagVM
	{
		[Required]
		public int Id { get; set; }
		public string? Name { get; set; }

	}
}
