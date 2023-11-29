﻿namespace BlogWebsite.Models
{
	public class ForumPost
	{
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
		public string? ApplicationUserId { get; set; }
		public ApplicationUser? ApplicationUsers { get; set; }
		public int TagId { get; set; }
		public Tag? Tag { get; set; }
		public string? Slug { get; set; }
		public ICollection<Comment>? Comments { get; set; }
	}
}
