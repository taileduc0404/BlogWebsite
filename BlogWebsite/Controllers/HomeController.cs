using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

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

			ViewData["keyword"] = keyword;

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

			var topicsWithPostCount = await _context.forumPosts!  //chứa thông tin số lượng bài post của tag theo TagId và gom nhóm dữ liệu từ bảng ForumPost
				.GroupBy(fpost => fpost.TopicId)
				.Select(group => new
				{
					TopicId = group.Key,
				})
				.ToListAsync();

			var topics = await _context.topics!.ToListAsync();

			foreach (var tag in topics)
			{
				var postCount = topicsWithPostCount.FirstOrDefault(t => t.TopicId == tag.Id);
			}

			vm.topics = topics;

			return View(vm);
		}


		[HttpGet("ForumPostTopic")]
		public async Task<IActionResult> ForumPostTopic(int id, int? page, string keyword)
		{
			int pageNumber = page ?? 1;
			int pageSize = 6;

			var vm = new HomeVM();
			var fpostsQuery = _context.forumPosts!.AsQueryable();

			if (!string.IsNullOrEmpty(keyword))
			{
				string lowerKeyword = keyword.ToLower();
				fpostsQuery = fpostsQuery.Where(p => p.Title!.ToLower().Contains(lowerKeyword));
				ViewBag.CurrentFilter = keyword; // Truyền từ khóa vào ViewBag
			}

			// Lấy danh sách ForumPosts
			var forumPosts = await fpostsQuery.Where(x => x.TopicId == id)
											  .OrderByDescending(x => x.CreatedDate)
											  .ToPagedListAsync(pageNumber, pageSize);

			// Tính số lượng comment cha cho mỗi ForumPost
			foreach (var forumPost in forumPosts)
			{
				// Lấy số lượng comment cha bằng cách đếm các comment có ParentCommentId là null
				var answerCount = _context.comments!.Count(c => c.ForumPostId == forumPost.Id && c.ParentCommentId == null);
				forumPost.AnswerCount = answerCount;
			}

			ViewBag.TopicId = id; // Truyền ID của topic vào ViewBag
			vm.forumPosts = forumPosts;

			return View(vm);
		}

		[HttpGet("About")]
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