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
		[HttpGet("ForumPost/{slug}")]
		public async Task<IActionResult> ForumPost(string slug)
		{
			var fpost = await _context.forumPosts!
				.Include(p => p.ApplicationUsers)
				.Include(t => t.Topic)
				.FirstOrDefaultAsync(x => x.Slug == slug);

			if (fpost == null)
			{
				_notification.Error("Post not found!");
				return RedirectToAction("NotFound");
			}

			fpost!.ViewCount++;
			await _context.SaveChangesAsync();

			var allAnswers = await _context.comments!
				.Where(c => c.ForumPostId == fpost!.Id)
				.Include(c => c.ApplicationUsers)
				.Include(c => c.Replies) // Bao gồm danh sách replies của mỗi comment
				.ToListAsync();
			var userId = _userManager.GetUserId(User);
			var myAnswer = allAnswers.Where(c => c.ApplicationUserId == userId).ToList();

			var vm = new ForumPostVM()
			{
				Id = fpost!.Id,
				Title = fpost.Title,
				ViewCount = fpost.ViewCount,
				TopicName = fpost.Topic != null ? fpost.Topic!.Name : "None Topic",
				AuthorName = fpost.ApplicationUsers != null ? fpost.ApplicationUsers.FirstName + " " + fpost.ApplicationUsers.LastName : "Unknown",
				CreatedDate = fpost.CreatedDate,
				Description = fpost.Description,
				Answers = allAnswers,
				MyAnswers = myAnswer
			};

			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> AddAnswer(int fpostId, string description)
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null || !User.Identity!.IsAuthenticated)
			{
				return RedirectToAction("Login", "User", new { area = "Admin" });
			}

			var post = await _context.forumPosts!
				.FirstOrDefaultAsync(p => p.Id == fpostId);

			var answer = new Comment
			{
				ForumPostId = fpostId,
				Description = description,
				ApplicationUserId = user.Id,
				CreatedDate = DateTime.Now
			};

			_context.comments!.Add(answer);
			await _context.SaveChangesAsync();

			return RedirectToAction("ForumPost", "ForumPost", new { slug = post!.Slug });
		}

		[HttpPost]
		public async Task<IActionResult> DeleteAnswer(int answerId, int postId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null || !User.Identity!.IsAuthenticated)
			{
				return RedirectToAction("Login", "User", new { area = "Admin" });
			}
			else
			{
				var answerToDelete = await _context.comments!
					.FirstOrDefaultAsync(c => c.Id == answerId || c.ParentCommentId == answerId);

				if (answerToDelete == null)
				{
					return RedirectToAction("Index", "Home");
				}

				_context.comments!.Remove(answerToDelete);
				await _context.SaveChangesAsync();

				return RedirectToAction("Index", "Home");
			}
		}
	}
}
