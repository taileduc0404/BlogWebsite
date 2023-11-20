namespace BlogWebsite.ViewModels
{
	public class BlogPostVM
	{
		public int Id { get; set; }
		public string? Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Description { get; set; }    
		public string? AuthorName { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? TagName { get; set; }

    }
}
