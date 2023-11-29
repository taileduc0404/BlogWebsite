using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class ForumController : Controller
	{

		public IActionResult Index()
		{
			return View();
		}
	}
}
