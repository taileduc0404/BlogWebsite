using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace BlogWebsite.Controllers
{
	public class HomeController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<HomeController> _logger;

		public HomeController(ApplicationDbContext context,
						ILogger<HomeController> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<IActionResult> Index(string keyword, int? page)
		{
			var vm = new HomeVM();

			IQueryable<Post> postsQuery = _context.posts!.Include(x => x.ApplicationUsers).OrderByDescending(x => x.CreatedDate);

			if (!string.IsNullOrEmpty(keyword))
			{
				string lowerKeyword = keyword.ToLower();
				postsQuery = postsQuery.Where(p => p.Title!.ToLower().Contains(lowerKeyword));
			}

			int pageNumber = page ?? 1;
			int pageSize = 6;

			vm.posts = await postsQuery.ToPagedListAsync(pageNumber, pageSize);

			ViewData["keyword"] = keyword;  // Pass the keyword to the view

			return View(vm);
		}
		[HttpGet("Tags")]
		public async Task<IActionResult> GetTags()
		{
			var vm = new HomeVM();
			IQueryable<Tag> tagsQuery = _context.tags!;

			vm.tags = await tagsQuery.ToListAsync();
			return View(vm);
		}

		[HttpGet("PostInTag")]
		public async Task<IActionResult> ShowPostInTag(int id, string keyword, int? page)
		{
			var vm = new HomeVM();

			IQueryable<Post> postsQuery = _context.posts!.Include(x => x.ApplicationUsers).Where(x => x.TagId == id).OrderByDescending(x => x.CreatedDate);

			if (!string.IsNullOrEmpty(keyword))
			{
				string lowerKeyword = keyword.ToLower();
				postsQuery = postsQuery.Where(p => p.Title!.ToLower().Contains(lowerKeyword));
			}

			int pageNumber = page ?? 1;
			int pageSize = 6;

			vm.posts = await postsQuery.ToPagedListAsync(pageNumber, pageSize);

			ViewData["keyword"] = keyword;  // Pass the keyword to the view

			return View(vm);
		}

		public IActionResult About()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}