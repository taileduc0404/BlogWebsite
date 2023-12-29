using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebsite.Controllers
{
	public class BlogController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		public INotyfService _notification { get; }

		public BlogController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
			INotyfService notification)
		{
			_context = context;
			_userManager = userManager;
			_notification = notification;
		}

		[HttpGet("[controller]/{slug}")]
		public async Task<IActionResult> Post(string slug)
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
				MyComments = myComment,
			};

			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> AddComment(int postId, string description)
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null || !User.Identity!.IsAuthenticated)
			{
				return RedirectToAction("Login", "User", new { area = "Admin" });
			}

			var post = await _context.posts!
				.FirstOrDefaultAsync(p => p.Id == postId);

			var comment = new Comment
			{
				PostId = postId,
				Description = description,
				ApplicationUserId = user.Id,
				CreatedDate = DateTime.Now
			};

			_context.comments!.Add(comment);
			await _context.SaveChangesAsync();

			return RedirectToAction("Post", "Blog", new { slug = post!.Slug });
		}

		[HttpPost]
		public async Task<IActionResult> AddReply(int parentId, int postId, string description)
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null || !User.Identity!.IsAuthenticated)
			{
				return RedirectToAction("Login", "User", new { area = "Admin" });
			}

			var post = await _context.posts!
				.FirstOrDefaultAsync(p => p.Id == postId);

			var parentComment = await _context.comments!
				.Include(c => c.Replies)
				.FirstOrDefaultAsync(c => c.Id == parentId);

			if (parentComment == null)
			{
				_notification.Error("Parent comment not found!");
				return RedirectToAction("NotFound", "Error");
			}

			var reply = new Comment
			{
				PostId = postId,
				ParentCommentId = parentId,
				Description = description,
				ApplicationUserId = user.Id,
				CreatedDate = DateTime.Now
			};

			_context.comments!.Add(reply);

			await _context.SaveChangesAsync();

			return RedirectToAction("Post", "Blog", new { slug = post!.Slug });
		}

		[HttpPost]
		public async Task<IActionResult> DeleteComment(int commentId, int postId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null || !User.Identity!.IsAuthenticated)
			{
				return RedirectToAction("Login", "User", new { area = "Admin" });
			}
			else
			{
				var commentToDelete = await _context.comments!
					.Include(c => c.Replies)
					.FirstOrDefaultAsync(c => c.Id == commentId || c.ParentCommentId == commentId);

				if (commentToDelete == null)
				{
					return RedirectToAction("Index", "Home");
				}

				_context.comments!.Remove(commentToDelete);
				await _context.SaveChangesAsync();

				return RedirectToAction("Index", "Home");
			}
		}


		public async Task<IActionResult> Status(int postId, bool isLike = true)
		{
			var user = await _userManager.GetUserAsync(User);

			var post = await _context.posts!.FirstOrDefaultAsync(x => x.Id == postId);

			if (isLike)
			{
				if (!post!.IsLike)
				{
					post.IsLike = true;
					post.CountLike++;
					post.CountDisLike--;
				}
				else
				{
					post.IsLike = false;
					post.CountLike--;
				}
			}
			else
			{
				if (post!.IsLike)
				{
					post.IsLike = false;
					post.CountLike--;
					post.CountDisLike++;
				}
				else
				{
					post.IsLike = true;
					post.CountDisLike++;
				}
			}

			_context.posts!.Update(post);
			_context.SaveChanges();
			return RedirectToAction("Post", "Blog", new { slug = post!.Slug });
		}

	}
}
