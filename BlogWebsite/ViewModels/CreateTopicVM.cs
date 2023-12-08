using System.ComponentModel.DataAnnotations;

namespace BlogWebsite.ViewModels
{
	public class CreateTopicVM
	{
		[Required]
		public int Id { get; set; }
		public string? Name { get; set; }
	}
}
