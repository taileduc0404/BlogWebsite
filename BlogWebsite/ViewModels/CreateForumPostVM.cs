using System.ComponentModel.DataAnnotations;

namespace BlogWebsite.ViewModels
{
	public class CreateForumPostVM
	{
		public int Id { get; set; }
		[Required]
		public string? Title { get; set; }
		public int TopicId { get; set; }
		public string? TopicName { get; set; }
		public string? ApplicationUserId { get; set; }
		public string? Description { get; set; }
	}
}
