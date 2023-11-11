using System.ComponentModel.DataAnnotations;

namespace BlogWebsite.ViewModels
{
	public class CreatPostVM
	{
		public int Id { get; set; }
		[Required]
		public string? Title { get; set; }
        public string? Tag { get; set; }
        public string? ApplicationUserId { get; set; }
		public string? Description { get; set; }
		public string? ThumbnailUrl { get; set; }
		public IFormFile? Thumbnail { get; set; }
	}
}
