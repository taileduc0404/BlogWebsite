using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.Utilites;
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
					TagName = x.Tag != null ? x.Tag.Name : "None Tag",
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
			var tag = await _context.tags!.FirstOrDefaultAsync(t => t.Name == vm.TagName);

			if (tag == null)
			{
				// Nếu tag chưa tồn tại, tạo mới tag trước khi tạo post
				tag = new Tag
				{
					Name = vm.TagName!.ToUpper()
				};

				_context.tags!.Add(tag);
				await _context.SaveChangesAsync();
			}

			var fpost = new ForumPost
			{
				Title = vm.Title,
				CreatedDate = DateTime.Now,
				Description = vm.Description,
				TagId = tag!.Id,
				ApplicationUserId = loggedInUser!.Id
			};

			if (tag == null)
			{
				tag = new Tag
				{
					Name = vm.TagName
				};

				_context.tags!.Add(tag);
				await _context.SaveChangesAsync();
			}

			if (fpost.Title != null)
			{
				string slug = vm.Title!.Trim();
				slug = slug.Replace(" ", "-");
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
			var post = await _context.forumPosts!.SingleOrDefaultAsync(x => x.Id == id);

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
				.Include(p => p.Tag)
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
				TagName = post.Tag != null ? post.Tag.Name : "",
				Description = post.Description,
			};

			return View(vm);
		}

		[HttpPost("EditForumPost")]
		public async Task<IActionResult> EditForumPost(CreateForumPostVM vm)
		{
			if (!ModelState.IsValid) { return View(vm); }
			var post = await _context.forumPosts!.SingleOrDefaultAsync(x => x.Id == vm.Id);
			var tag = await _context.tags!.SingleOrDefaultAsync(t => t.Name == vm.TagName);
			if (post == null)
			{
				_notification.Error("Post not found!");
				return View();
			}

			if (tag == null)
			{
				tag = new Tag()
				{
					Name = vm.TagName!.ToUpper()
				};
				_context.tags!.Add(tag);
				await _context.SaveChangesAsync();
			}

			post.Title = vm.Title;
			post.Tag = tag;
			post.Description = vm.Description;
			await _context.SaveChangesAsync();
			_notification.Success("Post Updated Successfully!");
			return RedirectToAction("Index", "Forum", new { area = "Admin" });

		}
	}
}
