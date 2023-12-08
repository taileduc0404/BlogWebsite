using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
			var setting = _context.settings!.ToList();
			vm.Title = setting[0].Title;
			vm.ShortDescription = setting[0].ShortDescription;
			vm.ThumbnailUrl = setting[0].ThumbnailUrl;

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

            var tagsWithPostCount = await _context.posts!  //chứa thông tin số lượng bài post của tag theo TagId và gom nhóm dữ liệu từ bảng post
                .GroupBy(post => post.TagId)
                .Select(group => new
                {
                    TagId = group.Key,
                    PostCount = group.Count()
                })
                .ToListAsync();

            var tags = await _context.tags!.ToListAsync();

            foreach (var tag in tags)
            {
                var postCount = tagsWithPostCount.FirstOrDefault(t => t.TagId == tag.Id)?.PostCount ?? 0;
                tag.PostCount = postCount;
            }

            vm.tags = tags;

            return View(vm);
        }

        [HttpGet("PostTag")]
		public async Task<IActionResult> PostTag(int id, string keyword, int? page)
		{
			int pageNumber = page ?? 1;
			int pageSize = 6;

			var vm = new HomeVM();
			var postsQuery = _context.posts!.AsQueryable();

			if (!string.IsNullOrEmpty(keyword))
			{
				string lowerKeyword = keyword.ToLower();
				postsQuery = postsQuery.Where(p => p.Title!.ToLower().Contains(lowerKeyword));
				ViewBag.CurrentFilter = keyword; // Truyền từ khóa vào ViewBag
			}

			postsQuery = postsQuery.Where(x => x.TagId == id).OrderByDescending(x => x.CreatedDate);

			ViewBag.TagId = id; // Truyền ID của tag vào ViewBag

			vm.posts = await postsQuery.ToPagedListAsync(pageNumber, pageSize);

			return View(vm);
		}

		[HttpGet("Forums")]
        public async Task<IActionResult> Forum(string keyword, int? page)
        {
            var vm = new HomeVM();
            var setting = _context.settings!.ToList();
            vm.Title = setting[0].Title;
            vm.ShortDescription = setting[0].ShortDescription;
            vm.ThumbnailUrl = setting[0].ThumbnailUrl;

            // Sửa truy vấn để bao gồm thông tin về Tag
            IQueryable<ForumPost> fpostsQuery = _context.forumPosts!
                .Include(x => x.ApplicationUsers)
                .Include(x => x.Topic) // Include thông tin về Tag
                .OrderByDescending(x => x.CreatedDate);

            if (!string.IsNullOrEmpty(keyword))
            {
                string lowerKeyword = keyword.ToLower();
                fpostsQuery = fpostsQuery.Where(p => p.Title!.ToLower().Contains(lowerKeyword));
            }

            int pageNumber = page ?? 1;
            int pageSize = 6;

            vm.forumPosts = await fpostsQuery.ToPagedListAsync(pageNumber, pageSize);

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