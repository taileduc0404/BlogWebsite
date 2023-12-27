using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.Utilites;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slugify;
using X.PagedList;

namespace BlogWebsite.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class ForumController : Controller
	{
		private readonly ApplicationDbContext _context;
		public INotyfService _notification { get; }
		private IWebHostEnvironment _webHostEnvironment;
		private readonly UserManager<ApplicationUser> _userManager;
		public ForumController(ApplicationDbContext context,
							  INotyfService notyfService,
							  IWebHostEnvironment webHostEnvironment,
							  UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_userManager = userManager;
			_notification = notyfService;
		}

		[HttpGet("ForumPost")]
		public async Task<IActionResult> Index(string keyword, int? page)
		{
			int pageNumber = page ?? 1;
			int pageSize = 4;

			var loggedInUser = await _userManager.GetUserAsync(User);
			var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

			var postsQuery = _context.forumPosts!
				.Where(x => loggedInUserRole[0] == WebsiteRole.WebisteAdmin || x.ApplicationUsers!.Id == loggedInUser!.Id)
				.OrderByDescending(x => x.CreatedDate)
				.Select(x => new ForumPostVM
				{
					Id = x.Id,
					Title = x.Title,
					TopicName = x.Topic != null ? x.Topic.Name : "None Topic",
					ViewCount = x.ViewCount,
					Description = x.Description,
					CreatedDate = x.CreatedDate,
					AuthorName = x.ApplicationUsers != null ? x.ApplicationUsers.FirstName + " " + x.ApplicationUsers.LastName : "Unknown Author"
				});

			if (!string.IsNullOrEmpty(keyword))
			{
				keyword = keyword.ToLower();
				postsQuery = postsQuery.Where(x => x.Title!.ToLower().Contains(keyword));
			}

			var listPost_Page = await postsQuery.ToPagedListAsync(pageNumber, pageSize);

			return View(listPost_Page);
		}

		[HttpGet("CreateForumPost")]
		public IActionResult CreateForumPost()
		{
			return View(new CreateForumPostVM());
		}

		[HttpPost("CreateForumPost")]
		public async Task<IActionResult> CreateForumPost(CreateForumPostVM vm)
		{
			if (!ModelState.IsValid) { return View(vm); }

			var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
			var topic = await _context.topics!.FirstOrDefaultAsync(t => t.Name == vm.TopicName);

			if (topic == null)
			{
				// Nếu tag chưa tồn tại, tạo mới tag trước khi tạo post
				topic = new Topic
				{
					Name = "None"
				};

				_context.topics!.Add(topic);
				await _context.SaveChangesAsync();
			}

			var fpost = new ForumPost
			{
				Title = vm.Title,
				CreatedDate = DateTime.Now,
				Description = vm.Description,
				TopicId = topic!.Id,
				ApplicationUserId = loggedInUser!.Id
			};

			if (topic == null)
			{
				topic = new Topic
				{
					Name = vm.TopicName
				};

				_context.topics!.Add(topic);
				await _context.SaveChangesAsync();
			}

			if (fpost.Title != null)
			{
				var slugHelper = new SlugHelper();
				string slug = slugHelper.GenerateSlug(vm.Title!.Trim());
				fpost.Slug = slug + "-" + Guid.NewGuid();
			}

			_context.forumPosts!.Add(fpost);
			await _context.SaveChangesAsync();
			_notification.Success("Post Created Successfully!");
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> DeleteForumPost(int id)
		{
			var post = await _context.forumPosts!.Include(x => x.Answer).SingleOrDefaultAsync(x => x.Id == id);

			var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
			var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

			if (loggedInUserRole[0] == WebsiteRole.WebisteAdmin || loggedInUser?.Id == post?.ApplicationUserId)
			{
				_context.forumPosts!.Remove(post!);
				await _context.SaveChangesAsync();
				_notification.Success("Post Deleted Successfully!");
				return RedirectToAction("Index", "Forum", new { area = "Admin" });
			}
			else
			{
				_notification.Error("You are not Authorized!");
				return RedirectToAction("Index", "Forum", new { area = "Admin" });
			}
		}

		[HttpGet("EditForumPost")]
		public async Task<IActionResult> EditForumPost(int id)
		{
			var post = await _context.forumPosts!
				.Include(p => p.Topic)
				.SingleOrDefaultAsync(x => x.Id == id);

			if (post == null)
			{
				_notification.Error("Post not found!");
				return View();
			}

			var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
			var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
			if (loggedInUserRole[0] != WebsiteRole.WebisteAdmin && loggedInUser!.Id != post.ApplicationUserId)
			{
				_notification.Error("You Are Not Author Of This Post!");
				return RedirectToAction("Index");
			}

			var vm = new CreateForumPostVM()
			{
				Id = post.Id,
				Title = post.Title,
				TopicName = post.Topic != null ? post.Topic.Name : "",
				Description = post.Description,
			};

			return View(vm);
		}

		[HttpPost("EditForumPost")]
		public async Task<IActionResult> EditForumPost(CreateForumPostVM vm)
		{
			if (!ModelState.IsValid) { return View(vm); }
			var post = await _context.forumPosts!.SingleOrDefaultAsync(x => x.Id == vm.Id);
			var topic = await _context.topics!.SingleOrDefaultAsync(t => t.Name == vm.TopicName);
			if (post == null)
			{
				_notification.Error("Post not found!");
				return View();
			}

			if (topic == null)
			{
				topic = new Topic()
				{
					Name = vm.TopicName!.ToUpper()
				};
				_context.topics!.Add(topic);
				await _context.SaveChangesAsync();
			}

			post.Title = vm.Title;
			post.Topic = topic;
			post.Description = vm.Description;
			await _context.SaveChangesAsync();
			_notification.Success("Post Updated Successfully!");
			return RedirectToAction("Index", "Forum", new { area = "Admin" });

		}
		[HttpGet]
		public IActionResult AutocompleteTopics(string keyword)
		{
			var tags = _context.topics!
				.Where(t => t.Name!.StartsWith(keyword))
				.Select(t => t.Name)
				.ToList();

			return Json(tags);
		}
	}
}
