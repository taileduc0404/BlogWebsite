using System.Collections.ObjectModel;

namespace BlogWebsite.Models
{
	public class Tag
	{
        public int Id { get; set; }
        public string? Name{ get; set; }
        public int PostCount { get; set; }
        public ICollection<Post>? posts { get; set; }
		public ICollection<ForumPost>? ForumPosts { get; set; }
	}
}
