namespace BlogWebsite.Models
{
	public class Reaction
	{
		public int Id { get; set; }
		public int PostId { get; set; }
		public string? UserId { get; set; }
		public bool IsLike { get; set; }
	}
}
