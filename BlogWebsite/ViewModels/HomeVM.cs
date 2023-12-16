using BlogWebsite.Models;
using X.PagedList;

namespace BlogWebsite.ViewModels
{
	public class HomeVM
	{
		public string? Title { get; set; }
		public string? ShortDescription { get; set; }
		public string? ThumbnailUrl { get; set; }
		public IPagedList<Post>? posts { get; set; }
		public IPagedList<ForumPost>? forumPosts { get; set; }
		public List<Tag>? tags { get; set; }
		public List<Topic>? topics { get; set; }
	}
}
