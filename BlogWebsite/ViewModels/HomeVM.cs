using BlogWebsite.Models;

namespace BlogWebsite.ViewModels
{
	public class HomeVM
	{
        public string? Title { get; set; }
        public string? ThumbnailUrl { get; set; }
        public List<Post>? posts { get; set; }
    }
}
