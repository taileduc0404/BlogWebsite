namespace BlogWebsite.Models
{
	public class ForumPost
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public DateTime CreatedDate { get; set; }
		public string? ApplicationUserId { get; set; }
		public ApplicationUser? ApplicationUsers { get; set; }
		public int ViewCount { get; set; }
		public int AnswerCount { get; set; }
		public int TopicId { get; set; }
		public Topic? Topic { get; set; }
		public string? Slug { get; set; }
		public ICollection<Comment>? Answer { get; set; }

	}
}
