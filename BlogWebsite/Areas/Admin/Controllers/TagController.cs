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
	public class TagController : Controller
	{
		private readonly ApplicationDbContext _context;
		public INotyfService _notification { get; }
		private IWebHostEnvironment _webHostEnvironment;
		private readonly UserManager<ApplicationUser> _userManager;
		public TagController(ApplicationDbContext context,
							  INotyfService notyfService,
							  IWebHostEnvironment webHostEnvironment,
							  UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_userManager = userManager;
			_notification = notyfService;
		}

		[HttpGet("Tag")]
		public async Task<IActionResult> Index(string keyword, int? page)
		{
			int pageNumber = page ?? 1;
			int pageSize = 6;

			var listOfTagVM = _context.tags!
				.Select(x => new TagVM
				{
					Id = x.Id,
					Name = x.Name ?? "None Tag" // Đặt tên mặt định cho Name nếu Name được set là null
				});

			if (!string.IsNullOrEmpty(keyword))
			{
				keyword = keyword.ToLower();
				listOfTagVM = listOfTagVM.Where(x => x.Name!.ToLower().Contains(keyword));

			}
			var listOfTag = await listOfTagVM.ToPagedListAsync(pageNumber, pageSize);
			return View(listOfTag);
		}

		[HttpGet("CreateTag")]
		public IActionResult CreateTag()
		{
			return View(new CreateTagVM());
		}

		[HttpPost("CreateTag")]
		public async Task<IActionResult> CreateTag(CreateTagVM vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

			var tagExist = await _context.tags!.AnyAsync(x => x.Name == vm.Name);

			if (tagExist)
			{
				_notification.Error("This Tag Has Already Exist!");
				return RedirectToAction("Index");
			}
			else
			{
				var tag = new Tag
				{
					Name = vm.Name!.ToUpper(),
				};

				_context.tags!.Add(tag);
				await _context.SaveChangesAsync();
				_notification.Success("Tag Created Successfully!");
				return RedirectToAction("Index");
			}
		}

		[HttpGet("PostInTag")]
		public async Task<IActionResult> ShowPostInTag(int id, int? page)
		{
			int pageNumber = page ?? 1;
			int pageSize = 4;

			var postsQuery = _context.posts!
				.Include(x => x.ApplicationUsers)
				.Include(t => t.Tag)
				.Where(x => x.TagId == id)
				.OrderByDescending(x => x.CreatedDate);

			ViewBag.TagId = id;

			var listOfPostVM = await postsQuery
				.Select(x => new PostVM
				{
					Id = x.Id,
					Title = x.Title,
					CreateDate = x.CreatedDate,
					ThumbnailUrl = x.ThumbnailUrl,
					AuthorName = x.ApplicationUsers != null ? x.ApplicationUsers.FirstName + " " + x.ApplicationUsers.LastName : "Unknown Author"
				})
				.ToPagedListAsync(pageNumber, pageSize);

			return View(listOfPostVM);
		}


		[HttpGet("GetTags")]
		public IActionResult GetTags(string term)
		{
			var tags = _context.tags!
				.Where(t => t.Name!.Contains(term, StringComparison.OrdinalIgnoreCase))
				.Select(t => new { name = t.Name })
				.ToList();

			return Json(tags);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteTag(int id)
		{
			var tag = await _context.tags!.Include(x => x.posts)!.ThenInclude(y => y.Comments).SingleOrDefaultAsync(x => x.Id == id);
			var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
			var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);


			if (tag != null)
			{
				foreach (var post in tag.posts!)
				{
					_context.comments!.RemoveRange(post.Comments!);
				}
				if (loggedInUserRole[0] != WebsiteRole.WebisteAdmin)
				{
					_notification.Error("You Are Not Author Of This Tag!");
					return RedirectToAction("Index", "Tag", new { area = "Admin" });
				}
				else
				{
					_context.posts!.RemoveRange(tag.posts!);
					_context.tags!.Remove(tag!);
					await _context.SaveChangesAsync();
					_notification.Success("Tag Deleted Successfully!");
				}
			}
			else
			{
				_notification.Success("Tag Not Found!");
			}
			return RedirectToAction("Index", "Tag", new { area = "Admin" });
		}
	}
}
