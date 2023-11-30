namespace BlogWebsite.ViewModels
{
	public class ForumPostVM
	{
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? AuthorName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Description { get; set; }
        public string? TagName { get; set; }
        public int ViewCount { get; set; }
        public int CommentCount { get; set; }

    }
}
