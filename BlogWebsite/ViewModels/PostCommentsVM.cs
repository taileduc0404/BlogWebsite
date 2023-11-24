namespace BlogWebsite.ViewModels
{
	public class PostCommentsVM
	{
		public int PostId { get; set; }
		public string? PostTitle { get; set; }
		public List<CommentVM>? Comments { get; set; }
	}
}
