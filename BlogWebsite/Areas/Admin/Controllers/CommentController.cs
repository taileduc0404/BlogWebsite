using BlogWebsite.Data;
using BlogWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebsite.Areas.Admin.Controllers
{
	public class CommentController : Controller
	{
		private readonly ApplicationDbContext _context;
        public CommentController(ApplicationDbContext context)
        {
			_context = context;
        }
        public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateComment(int postId, string description)
		{
			if (!User.Identity!.IsAuthenticated)
			{
				// nếu người comment chưa đăng nhập thì chuyển sang trang đăng nhập
				return RedirectToAction("Login", "User");
			}

			var post = await _context.posts!.FirstOrDefaultAsync(p => p.Id == postId);
			if (post == null)
			{
				// trả về kết quả NotFound nếu không có bài post trong db
				return NotFound();
			}

			var comment = new Comment
			{
				PostId = postId,
				Description = description
			};

			_context.comments!.Add(comment);
			await _context.SaveChangesAsync();

			return RedirectToAction("Post", "Blog",  new { id = postId, area="Default"});
		}

		[HttpPost]
		public async Task<IActionResult> AddReply(int parentCommentId, string description)
		{
			if (!User.Identity!.IsAuthenticated)
			{
				return RedirectToAction("Login", "Account");
			}

			var parentComment = await _context.comments!.FirstOrDefaultAsync(c => c.Id == parentCommentId);
			if (parentComment == null)
			{
				return NotFound();
			}

			var reply = new Comment
			{
				ParentCommentId = parentCommentId,
				Description = description
			};

			_context.comments!.Add(reply);
			await _context.SaveChangesAsync();

			return RedirectToAction("Details", "Post", new { id = parentComment.PostId });
		}

	}
}
