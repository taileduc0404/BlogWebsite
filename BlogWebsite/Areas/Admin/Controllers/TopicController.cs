using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace BlogWebsite.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class TopicController : Controller
	{
		private readonly ApplicationDbContext _context;
		public INotyfService _notification { get; }
		private IWebHostEnvironment _webHostEnvironment;
		private readonly UserManager<ApplicationUser> _userManager;
		public TopicController(ApplicationDbContext context,
							  INotyfService notyfService,
							  IWebHostEnvironment webHostEnvironment,
							  UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_userManager = userManager;
			_notification = notyfService;
		}

		[HttpGet("Topic")]
		public async Task<IActionResult> Index(string keyword)
		{
			var listOfTopic = await _context.topics!
				.ToListAsync();

			var listOfTopicVM = listOfTopic
				.Select(x => new TopicVM
				{
					Id = x.Id,
					Name = x.Name ?? "None Topic" // Đặt tên mặt định cho Name nếu Name được set là null
				})
				.ToList();

			if (string.IsNullOrEmpty(keyword))
			{
				return View(listOfTopicVM);
			}
			else
			{
				return View(listOfTopicVM.Where(x => x.Name!.ToLower().Contains(keyword)));
			}
		}

		[HttpGet("CreateTopic")]
		public IActionResult CreateTopic()
		{
			return View(new CreateTopicVM());
		}

		[HttpPost("CreateTopic")]
		public async Task<IActionResult> CreateTopic(CreateTagVM vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

			var topicExist = await _context.topics!.AnyAsync(x => x.Name == vm.Name);

			if (topicExist)
			{
				_notification.Error("This Topic Has Already Exist!");
				return RedirectToAction("Index", "Topic", new { area = "Admin" });
			}
			else
			{
				var topic = new Topic
				{
					Name = vm.Name!.ToUpper(),
				};

				_context.topics!.Add(topic);
				await _context.SaveChangesAsync();
				_notification.Success("Topic Created Successfully!");
				return RedirectToAction("Index", "Topic", new { area = "Admin" });
			}
		}

		[HttpGet("ForumPostInTopic")]
		public async Task<IActionResult> ShowForumPostInTopic(int id, int? page)
		{
			int pageNumber = page ?? 1;
			int pageSize = 4;

			var postsQuery = _context.forumPosts!
				.Include(x => x.ApplicationUsers)
				.Include(t => t.Topic)
				.Where(x => x.TopicId == id)
				.OrderByDescending(x => x.CreatedDate);

			ViewBag.TopicId = id;

			var listOfPostVM = await postsQuery
				.Select(x => new ForumPostVM
				{
					Id = x.Id,
					Title = x.Title,
					CreatedDate = x.CreatedDate,
					AuthorName = x.ApplicationUsers != null ? x.ApplicationUsers.FirstName + " " + x.ApplicationUsers.LastName : "Unknown Author"
				})
				.ToPagedListAsync(pageNumber, pageSize);

			return View(listOfPostVM);
		}

		[HttpGet("GetTopics")]
		public IActionResult GetTopics(string term)
		{
			var topics = _context.topics!
				.Where(t => t.Name!.Contains(term, StringComparison.OrdinalIgnoreCase))
				.Select(t => new { name = t.Name })
				.ToList();

			return Json(topics);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteTopic(int id)
		{
			var topic = await _context.topics!.Include(x => x.ForumPosts)!.ThenInclude(y => y.Answer).SingleOrDefaultAsync(x => x.Id == id);

			if (topic != null)
			{

				foreach (var post in topic.ForumPosts!)
				{
					_context.comments!.RemoveRange(post.Answer!);
				}
				_context.forumPosts!.RemoveRange(topic.ForumPosts); // Remove ForumPosts
				_context.topics!.Remove(topic!);
				await _context.SaveChangesAsync();
				_notification.Success("Topic Deleted Successfully!");
			}
			else
			{
				_notification.Success("Topic Not Found!");

			}

			return RedirectToAction("Index", "Topic", new { area = "Admin" });
		}
	}
}
