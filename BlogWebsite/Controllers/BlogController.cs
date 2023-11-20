using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebsite.Controllers
{
	public class BlogController : Controller
	{
		private readonly ApplicationDbContext _context;
		public INotyfService _notification { get; }

		public BlogController(ApplicationDbContext context, INotyfService notification)
		{
			_context = context;
			_notification = notification;
		}
		[HttpGet("[controller]/{slug}")]
		public IActionResult Post(string slug)
		{
			var post = _context.posts!.Include(x => x.ApplicationUsers).FirstOrDefault(x => x.Slug == slug);
			if (slug == "")
			{
				_notification.Error("Post not found!");
				return View();
			}
			if (post == null)
			{
				_notification.Error("Post not found!");
				return View();
			}

			var vm = new BlogPostVM()
			{
				Id = post!.Id,
				Title = post.Title,
				AuthorName = post.ApplicationUsers!.FirstName + " " + post.ApplicationUsers.LastName,
				CreatedDate = post.CreatedDate,
				ThumbnailUrl = post.ThumbnailUrl,
				Description = post.Description,
			};
			return View(vm);
		}
	}
}
