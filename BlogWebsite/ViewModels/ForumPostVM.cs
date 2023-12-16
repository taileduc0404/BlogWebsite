using BlogWebsite.Models;

namespace BlogWebsite.ViewModels
{
	public class ForumPostVM
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public string? AuthorName { get; set; }
		public DateTime CreatedDate { get; set; }
		public string? Description { get; set; }
		public string? TopicName { get; set; }
		public int ViewCount { get; set; }
		public int AnswerCount { get; set; }

		public List<Comment>? Answers { get; set; }
		public List<Comment>? MyAnswers { get; set; }

		public ForumPostVM()
		{
			Answers = new List<Comment>();
			MyAnswers = new List<Comment>(); // Khởi tạo danh sách MyAnswers
		}
	}
}
