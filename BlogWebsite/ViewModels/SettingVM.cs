using System.ComponentModel.DataAnnotations;

namespace BlogWebsite.ViewModels
{
	public class SettingVM
	{
		public int Id { get; set; }
		[Required]
		public string? SiteName { get; set; }
		public string? Title { get; set; }
		public string? ShortDescription { get; set; }
		public string? ThumbnailUrl { get; set; }
		public IFormFile? Thumbnail { get; set; }
	}
}
