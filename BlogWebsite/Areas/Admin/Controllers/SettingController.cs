using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebsite.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	[Authorize]
	public class SettingController : Controller
	{
		private readonly ApplicationDbContext _context;
		public INotyfService _notification { get; }
		private IWebHostEnvironment _webHostEnvironment;
		public SettingController(ApplicationDbContext context,
								 INotyfService notification,
								 IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_notification = notification;
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpGet("Setting")]
		public async Task<IActionResult> Index()
		{
			var settings = _context.settings!.ToList();

			if (settings.Count > 0)
			{
				var vm = new SettingVM()
				{
					Id = settings[0].Id,
					SiteName = settings[0].SiteName,
					Title = settings[0].Title,
					ShortDescription = settings[0].ShortDescription,
					ThumbnailUrl = settings[0].ThumbnailUrl,
				};
				return View(vm);
			}
			var setting = new Setting()
			{
				SiteName = "Demo Name"
			};

			await _context.settings!.AddRangeAsync(setting);
			await _context.SaveChangesAsync();

			var createdSettings = _context.settings!.ToList();
			var createdVm = new SettingVM()
			{
				Id = createdSettings[0].Id,
				SiteName = createdSettings[0].SiteName,
				Title = createdSettings[0].Title,
				ShortDescription = createdSettings[0].ShortDescription,
				ThumbnailUrl = createdSettings[0].ThumbnailUrl,
			};

			return View(createdVm);
		}

		[HttpPost("Setting")]
		public async Task<IActionResult> Index(SettingVM vm)
		{
			if (!ModelState.IsValid) { return View(vm); }
			var settings = await _context.settings!.FirstOrDefaultAsync(x => x.Id == vm.Id);
			if (settings == null)
			{
				_notification.Error("Something went wrong!");
				return View(vm);
			}

			settings.SiteName = vm.SiteName;
			settings.Title = vm.Title;
			settings.ShortDescription = vm.ShortDescription;
			if (vm.Thumbnail != null)
			{
				settings.ThumbnailUrl = UploadImage(vm.Thumbnail);
			}
			await _context.SaveChangesAsync();
			_notification.Success("Setting Updated Successfully!");
			return RedirectToAction("Index", "Setting", new { area = "Admin" });

		}

		public string UploadImage(IFormFile file)
		{
			string uniqueFileName = "";
			var folerPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails");
			uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
			var filePath = Path.Combine(folerPath, uniqueFileName);
			using (FileStream fileStream = System.IO.File.Create(filePath))
			{
				file.CopyTo(fileStream);
			}
			return uniqueFileName;
		}
	}
}
