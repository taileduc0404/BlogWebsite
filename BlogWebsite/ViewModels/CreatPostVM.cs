using System.ComponentModel.DataAnnotations;

namespace BlogWebsite.ViewModels
{
	public class CreatPostVM
	{
		public int Id { get; set; }
		[Required]
		public string? Title { get; set; }
		public int TagId { get; set; }
		public string? TagName { get; set; }
		public string? ApplicationUserId { get; set; }
		public string? Description { get; set; }
		public string? ThumbnailUrl { get; set; }
		public IFormFile? Thumbnail { get; set; }
	}
}
