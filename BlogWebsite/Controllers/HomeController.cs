using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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

        public async Task<IActionResult> Index(int? page, string keyword)
        {
			var vm = new HomeVM();
			//var setting = _context.Settings!.ToList();
			//vm.Title = setting[0].Title;
			//vm.ThumbnailUrl = setting[0].ThumbnailUrl;
			//int pageSize = 4;
			int pageNumber = (page ?? 1);
			if (string.IsNullOrEmpty(keyword))
			{
                vm.posts = await _context.posts!.Include(x => x.ApplicationUsers).OrderByDescending(x => x.CreatedDate).ToListAsync();
			}
			else
			{
				keyword = keyword.ToLower();
                vm.posts = await _context.posts!.Where(p => p.Title!.ToLower().Contains(keyword)).Include(x => x.ApplicationUsers).OrderByDescending(x => x.CreatedDate).ToListAsync();
			}
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