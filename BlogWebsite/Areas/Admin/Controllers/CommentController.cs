using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.Utilites;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

		[HttpGet("PostAndComment")]
		public async Task<IActionResult> Index(string keyword)
		{
			var loggedInUser = await _userManager.GetUserAsync(User);
			var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

			var listOfPosts = await _context.posts!
				.Include(x => x.ApplicationUsers)
				.Include(t => t.Tag)    //  Lấy ra thẻ tag của bài post
				.OrderByDescending(x => x.CreatedDate)
				.Where(x => loggedInUserRole[0] == WebsiteRole.WebisteAdmin || x.ApplicationUsers!.Id == loggedInUser!.Id)
				.ToListAsync();

			var listOfPostVM = listOfPosts.Select(x => new PostVM
			{
				Id = x.Id,
				Title = x.Title,
				TagName = x.Tag != null ? x.Tag.Name : "None Tag",
				ViewCount = x.ViewCount,
				CreateDate = x.CreatedDate,
				ThumbnailUrl = x.ThumbnailUrl,
				AuthorName = x.ApplicationUsers != null ? x.ApplicationUsers.FirstName + " " + x.ApplicationUsers.LastName : "Unknown Author"
			}).ToList();

			if (string.IsNullOrEmpty(keyword))
			{
				return View(listOfPostVM);

			}
			else
			{
				return View(listOfPostVM.Where(x => x.Title!.ToLower().Contains(keyword)));
			}
		}

		//public async Task<IActionResult> ShowCommetInPost()
		//{

		//}

	}
}
