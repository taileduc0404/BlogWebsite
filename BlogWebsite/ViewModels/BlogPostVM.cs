using BlogWebsite.Models;

namespace BlogWebsite.ViewModels
{
	public class BlogPostVM
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public DateTime CreatedDate { get; set; }
		public int ViewCount { get; set; }
		public string? Description { get; set; }
		public string? AuthorName { get; set; }
		public string? ThumbnailUrl { get; set; }
		public string? TagName { get; set; }
		public int? TagId { get; set; }

		public int LikeCount { get; set; }

		public List<Comment> Comments { get; set; }
		public List<Comment> MyComments { get; set; } // Danh sách các comment của người dùng hiện tại
		public List<Reaction> Reactions { get; set; }

		public BlogPostVM()
		{
			Comments = new List<Comment>();
			MyComments = new List<Comment>(); // Khởi tạo danh sách MyComments
			Reactions = new List<Reaction>();
		}
	}
}
