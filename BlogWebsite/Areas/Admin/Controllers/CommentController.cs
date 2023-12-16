using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.Utilites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace BlogWebsite.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class CommentController : Controller
	{
		private readonly ApplicationDbContext _context;
		public INotyfService _notification { get; }
		private IWebHostEnvironment _webHostEnvironment;
		private readonly UserManager<ApplicationUser> _userManager;
		public CommentController(ApplicationDbContext context,
							  INotyfService notyfService,
							  IWebHostEnvironment webHostEnvironment,
							  UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_userManager = userManager;
			_notification = notyfService;
		}

		[HttpGet("PostComment")]
		public async Task<IActionResult> Index(string keyword, int? page)
		{
			int pageNumber = page ?? 1;
			int pageSize = 6;

			var loggedInUser = await _userManager.GetUserAsync(User);
			var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser);

			var postsQuery = _context.posts!
				.Where(x => loggedInUserRole[0] == WebsiteRole.WebisteAdmin || x.ApplicationUserId == loggedInUser.Id)
				.AsQueryable(); // Chuyển danh sách bài post sang IQueryable để bắt đầu truy vấn

			if (!string.IsNullOrEmpty(keyword))
			{
				keyword = keyword.Trim().ToLower(); // Chuyển từ khóa nhập vào về dạng chữ thường và loại bỏ khoảng trắng ở đầu và cuối
				postsQuery = postsQuery.Where(x => x.Title != null && x.Title.ToLower().Contains(keyword));
			}

			postsQuery = postsQuery.OrderByDescending(x => x.CreatedDate); // Sắp xếp sau khi áp dụng bộ lọc

			var posts = await postsQuery.ToPagedListAsync(pageNumber, pageSize);
			ViewBag.CurrentSort = "";
			ViewBag.CurrentFilter = keyword;
			return View(posts);
		}


		[HttpGet("CommentList")]
		public async Task<IActionResult> Comments(int postId)
		{
			var post = await _context.posts!
				.Include(p => p.Comments)!
				.ThenInclude(c => c.ApplicationUsers)
				.SingleOrDefaultAsync(p => p.Id == postId);

			if (post == null)
			{
				return NotFound();
			}

			return View(post);
		}
	}
}
