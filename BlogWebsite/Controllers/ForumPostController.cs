using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebsite.Controllers
{
	public class ForumPostController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		public INotyfService _notification { get; }

		public ForumPostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
			INotyfService notification)
		{
			_context = context;
			_userManager = userManager;
			_notification = notification;
		}
		[HttpGet("[controller]/{slug}")]
		public async Task<IActionResult> ForumPost(string slug)
		{
			var post = await _context.posts!
				.Include(p => p.ApplicationUsers)
				.Include(t => t.Tag)
				.FirstOrDefaultAsync(x => x.Slug == slug);

			if (post == null)
			{
				_notification.Error("Post not found!");
				return RedirectToAction("NotFound");
			}

			post.ViewCount++;
			await _context.SaveChangesAsync();

			var allComments = await _context.comments!
				.Where(c => c.PostId == post.Id)
				.Include(c => c.ApplicationUsers)
				.Include(c => c.Replies) // Bao gồm danh sách replies của mỗi comment
				.ToListAsync();
			var userId = _userManager.GetUserId(User);
			var myComment = allComments.Where(c => c.ApplicationUserId == userId).ToList();

			var vm = new BlogPostVM()
			{
				Id = post.Id,
				Title = post.Title,
				ViewCount = post.ViewCount,
				TagName = post.Tag != null ? post.Tag.Name : "None Tag",
				AuthorName = post.ApplicationUsers != null ? post.ApplicationUsers.FirstName + " " + post.ApplicationUsers.LastName : "Unknown",
				CreatedDate = post.CreatedDate,
				ThumbnailUrl = post.ThumbnailUrl,
				Description = post.Description,
				Comments = allComments,
				MyComments = myComment
			};

			return View(vm);
		}
	}
}
