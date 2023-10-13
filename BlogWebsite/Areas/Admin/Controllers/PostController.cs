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
	public class PostController : Controller
	{
		private readonly ApplicationDbContext _context;
		public INotyfService _notification { get; }
		private IWebHostEnvironment _webHostEnvironment;
		private readonly UserManager<ApplicationUser> _userManager;
		public PostController(ApplicationDbContext context,
							  INotyfService notyfService,
							  IWebHostEnvironment webHostEnvironment,
							  UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_userManager = userManager;
			_notification = notyfService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var listOfPost = new List<Post>();
			var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
			var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
			if (loggedInUserRole[0] == WebsiteRole.WebisteAdmin)
			{
				listOfPost = await _context.posts!.Include(x => x.ApplicationUsers).ToListAsync();
			}
			else
			{
				listOfPost = await _context.posts!.Include(x => x.ApplicationUsers).Where(x => x.ApplicationUsers!.Id == loggedInUser!.Id).ToListAsync();
			}

			var listOfPostVM = listOfPost.Select(x => new PostVM()
			{
				Id = x.Id,
				Title = x.Title,
				AuthorName = x.ApplicationUsers!.FirstName + x.ApplicationUsers.LastName,
				CreateDate = x.CreatedDate,
				ThumbnailUrl = x.ThumbnailUrl
			});


			return View(listOfPostVM);
		}


		[HttpGet]
		public IActionResult Create()
		{
			return View(new CreatPostVM());
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreatPostVM vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

			var post = new Post()
			{
				Title = vm.Title,
				Description = vm.Description,
				ApplicationUserId = loggedInUser!.Id
			};

			if (post.Title != null)
			{
				string slug = vm.Title!.Trim();
				slug = slug.Replace(" ", "-");
				post.Slug = slug + "-" + Guid.NewGuid();
			}

			if (post.ThumbnailUrl != null)
			{
				post.ThumbnailUrl = UploadImage(vm.Thumbnail!);
			}

			await _context.posts!.AddAsync(post);
			await _context.SaveChangesAsync();
			_notification.Success("Create Post Successfully!");

			return RedirectToAction("Index", "Post", new { area = "Admin" });
		}
		public string UploadImage(IFormFile file)
		{
			string uniqueFileName = "";
			var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails");
			uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
			var filePath = Path.Combine(folderPath, uniqueFileName);
			using (FileStream fileStream = System.IO.File.Create(filePath))
			{
				file.CopyTo(fileStream);
			}
			return uniqueFileName;
		}
	}
}
