namespace BlogWebsite.Models
{
	public class Comment
	{
		public int Id { get; set; }
		public string? Description { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public int? PostId { get; set; } // Khóa ngoại tham chiếu đến Post
		public Post? Post { get; set; }

		public int? ForumPostId { get; set; }
		public ForumPost? ForumPost { get; set; }

		public string? ApplicationUserId { get; set; }  // Khóa ngoại tham chiếu đến ApplicationUser
		public ApplicationUser? ApplicationUsers { get; set; }
		public int? ParentCommentId { get; set; } // comment cha (xác định cho reply)
		public Comment? ParentComment { get; set; } // navigation property tham chiếu đến comment cha

		public List<Comment> Replies { get; set; } // Danh sách các reply

		public Comment()
		{
			Replies = new List<Comment>();
		}
	}
}
