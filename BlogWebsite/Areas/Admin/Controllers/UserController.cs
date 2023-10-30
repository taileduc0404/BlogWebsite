using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Models;
using BlogWebsite.Utilites;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Text;
using MailKit.Net.Smtp;

namespace BlogWebsite.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class UserController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly INotyfService _notification;
        public UserController(UserManager<ApplicationUser> userManager,
								SignInManager<ApplicationUser> signInManager,
								INotyfService notyfService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_notification = notyfService;
		}
        //Index
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var vm = users.Select(x => new UserVM()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                Email = x.Email,
            }).ToList();
            foreach (var user in vm)
            {
                var singleUser = await _userManager.FindByIdAsync(user.Id);
                var role = await _userManager.GetRolesAsync(singleUser);
                user.Role = role.FirstOrDefault();
            }

            return View(vm);
        }


        //Register
        [HttpGet("Register")]
		public IActionResult Register()
		{
			return View(new RegisterVM());
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register(RegisterVM vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			var checkUserByEmail = await _userManager.FindByEmailAsync(vm.Email);
			if (checkUserByEmail != null)
			{
				_notification.Error("This Email Already Exist!");
				return View(vm);
			}
			var checkUserByUsername = await _userManager.FindByNameAsync(vm.UserName);
			if (checkUserByUsername != null)
			{
				_notification.Error("This UserName Already Exist!");
				return View(vm);
			}

			var applicationUser = new ApplicationUser()
			{
				FirstName = vm.FirstName,
				LastName = vm.LastName,
				Email = vm.Email,
				UserName = vm.UserName
			};

			var result = await _userManager.CreateAsync(applicationUser, vm.Password);

			if (result.Succeeded)
			{
				if (vm.IsAuthor)
				{
					await _userManager.AddToRoleAsync(applicationUser, WebsiteRole.WebisteAuthor);
				}
				else
				{
					await _userManager.AddToRoleAsync(applicationUser, WebsiteRole.WebisteReader);
				}
				_notification.Success("User Created Successfully!");
				return RedirectToAction("Login", "User", new { area = "Admin" });
			}
			return View(vm);

		}



		[Authorize]
		[HttpGet]
		public async Task<IActionResult> ResetPassword(string id)
		{
			var existingUser = await _userManager.FindByIdAsync(id);
			if (existingUser == null)
			{
				_notification.Error("User doesn't exist!");
				return View();
			}
			var vm = new ResetPasswordVM()
			{
				Id = existingUser.Id,
				Username = existingUser.UserName
			};
			return View(vm);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
		{
			if (!ModelState.IsValid) { return View(vm); }
			var existingUser = await _userManager.FindByIdAsync(vm.Id);
			if (existingUser == null)
			{
				_notification.Error("User doesn't exist!");
				return View(vm);
			}
			var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
			var result = await _userManager.ResetPasswordAsync(existingUser, token, vm.NewPassword);
			if (result.Succeeded)
			{
				_notification.Success("Password reset successful!");
				return RedirectToAction(nameof(Index));
			}
			return View(vm);
		}


        //Login
        [HttpGet("Login")]
		public IActionResult Login()
		{
			if (!HttpContext.User.Identity!.IsAuthenticated)
			{
				return View(new LoginVM());
			}
			return RedirectToAction("Index", "Post", new { area = "Admin" });
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginVM vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}
			var existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == vm.Username);
			if (existingUser == null)
			{
				_notification.Error("Username does not exist");
				return View(vm);
			}
			var verifyPassword = await _userManager.CheckPasswordAsync(existingUser, vm.Password);
			if (!verifyPassword)
			{
				_notification.Error("Password does not match");
				return View(vm);
			}
			await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, true);
			//if (result.Succeeded)
			//{
			//	_notification.Success("Logged In Successfully!");
			//	return RedirectToAction("Index", "Post", new { area = "Admin" });
			//}
			//else
			//{
			//	_notification.Error("Account has not been verified");
			//	return View(vm);
			//}
			_notification.Success("Logged In Successfully!");
			return RedirectToAction("Index", "Home", new { area = "Default" });

		}

		[HttpPost]
		[Authorize]
		public IActionResult Logout()
		{
			_signInManager.SignOutAsync();
			_notification.Success("You logged out successfully!");
			return RedirectToAction("Index", "Home", new { area = "Default" });
		}
	}
}
