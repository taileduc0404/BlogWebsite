namespace BlogWebsite.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUsers { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int ViewCount { get; set; }
        public string? Description { get; set; }
        public List<Tag>? Tags { get; set; } = new List<Tag>();
        public string? Slug { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
