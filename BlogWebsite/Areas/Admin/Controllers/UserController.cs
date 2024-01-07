using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.Utilites;
using BlogWebsite.ViewModels;
using EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogWebsite.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class UserController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly INotyfService _notification;
		private readonly IEmailSender _emailSender;
		private readonly EmailConfiguration _emailConfig;

		public UserController(UserManager<ApplicationUser> userManager,
							  SignInManager<ApplicationUser> signInManager,
							  INotyfService notyfService, IEmailSender emailSender,
							  EmailConfiguration emailConfig,
							  ApplicationDbContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_notification = notyfService;
			_emailSender = emailSender;
			_emailConfig = emailConfig;
			_context = context;
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("User")]
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
				IsLocked = x.IsLocked,

			}).ToList();
			foreach (var user in vm)
			{
				var singleUser = await _userManager.FindByIdAsync(user.Id);
				var role = await _userManager.GetRolesAsync(singleUser);
				user.Role = role.FirstOrDefault();
				user.IsLocked = await _userManager.IsLockedOutAsync(singleUser);
			}

			return View(vm);
		}

		[HttpGet("ForgotPassword")]
		public IActionResult ForgotPassword()
		{
			return View(new ForgotPasswordVM());
		}

		[HttpPost("ForgotPassword")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordVM vm)
		{
			if (!ModelState.IsValid)
				return View(vm);
			var user = await _userManager.FindByEmailAsync(vm.Email);
			var checkUserByEmail = await _userManager.FindByEmailAsync(vm.Email);
			if (user == null)
			{
				return RedirectToAction(nameof(ForgotPasswordConfirmationError));
			}
			else
			{
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				var callback = Url.Action("ResetPassword", "User", new { token, email = user.Email }, Request.Scheme);
				var message = new Message(_emailConfig, new string[] { user.Email }, "Reset password link", callback!, null!);
				await _emailSender.SendEmailAsync(message);
				return RedirectToAction(nameof(ForgotPasswordConfirmation));
			}

		}

		[HttpGet("ForgotPasswordConfirmation")]
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		[HttpGet("ForgotPasswordConfirmationError")]
		public IActionResult ForgotPasswordConfirmationError()
		{
			return View();
		}

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
				await _userManager.AddToRoleAsync(applicationUser, WebsiteRole.WebisteAuthor);
				_notification.Success("User Created Successfully!");
				return RedirectToAction("Login", "User", new { area = "Admin" });
			}
			return View(vm);

		}

		[HttpGet("ResetPassword")]
		public IActionResult ResetPassword(string token, string email)
		{
			var model = new ResetPasswordVM { Token = token, Email = email };
			return View(model);
		}

		[HttpPost("ResetPassword")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
		{
			if (!ModelState.IsValid)
				return View(vm);

			var user = await _userManager.FindByEmailAsync(vm.Email);
			if (user == null)
				RedirectToAction(nameof(ResetPasswordConfirmation));

			var resetPassResult = await _userManager.ResetPasswordAsync(user!, vm.Token, vm.Password);
			if (!resetPassResult.Succeeded)
			{
				foreach (var error in resetPassResult.Errors)
				{
					ModelState.TryAddModelError(error.Code, error.Description);
				}

				return View();
			}

			return RedirectToAction(nameof(ResetPasswordConfirmation));
		}

		[HttpGet("ResetPasswordConfirmation")]
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		[HttpGet("Login")]
		public IActionResult Login()
		{
			if (!HttpContext.User.Identity!.IsAuthenticated)
			{
				return View(new LoginVM());
			}
			return RedirectToAction("Index", "Home", new { area = "Default" });
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginVM vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			var user = await _userManager.FindByEmailAsync(vm.Username);
			if (user == null)
			{
				user = await _userManager.FindByNameAsync(vm.Username);
				if (user == null)
				{
					_notification.Error("This account does not exist.");
					return View(vm);
				}
			}

			var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, vm.Password, vm.RememberMe, true);

			if (user.IsLocked)
			{
				_notification.Error("Your Account Is Locked!");
				return View(vm);
			}
			else if (signInResult.IsLockedOut)
			{
				_notification.Error("Your Account Is Locked!");
				return View(vm);
			}
			else if (signInResult.IsNotAllowed)
			{
				_notification.Error("Your Account Is Not Allowed to Sign In.");
				return View(vm);
			}
			else if (signInResult.Succeeded)
			{
				_notification.Success("Logged In Successfully!");
				return RedirectToAction("Index", "Home", new { area = "Default" });
			}
			else
			{
				_notification.Error("Invalid Password");
				return View(vm);
			}
		}

		[HttpPost]
		public async Task<IActionResult> DeleteUser(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				_notification.Error("This User Is Not Exist!");
				return RedirectToAction("Index", "User", new { area = "Admin" });
			}

			var post = await _context.posts!.Where(p => p.ApplicationUserId == userId).Include(p => p.Comments).ToListAsync();
			if (post.Count > 0)
			{
				_context.posts!.RemoveRange(post);
				await _context.SaveChangesAsync();
			}
			var fpost = await _context.forumPosts!.Where(p => p.ApplicationUserId == userId).Include(p => p.Answer).ToListAsync();
			if (fpost.Count > 0)
			{
				_context.forumPosts!.RemoveRange(fpost);
				await _context.SaveChangesAsync();
			}

			var commment = await _context.comments!.Where(c => c.ApplicationUserId == userId).ToListAsync();
			if (commment.Count > 0)
			{
				_context.comments!.RemoveRange(commment);
				await _context.SaveChangesAsync();
			}

			var result = await _userManager.DeleteAsync(user!);
			if (result.Succeeded)
			{
				_notification.Success("Delete User Successfully!");
				return RedirectToAction("Index", "User", new { area = "Admin" });

			}
			else
			{
				_notification.Error("Delete User Fail!");
				return RedirectToAction("Index", "User", new { area = "Admin" });


			}
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> LockUser(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				_notification.Error("This User Is Not Exist!");
				return RedirectToAction("Index", "User", new { area = "Admin" });
			}


			user.LockoutEnd = DateTimeOffset.MaxValue;
			var result = await _userManager.UpdateAsync(user);

			if (result.Succeeded)
			{
				_notification.Success("Lock User Successfully!");
				user.IsLocked = true;
			}
			else
			{
				_notification.Error("Lock User Fail!");
			}
			return RedirectToAction("Index", "User", new { area = "Admin" });
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UnlockUser(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				_notification.Error("This User Is Not Exist!");
				return RedirectToAction("Index", "User", new { area = "Admin" });
			}

			user.LockoutEnd = null;
			var result = await _userManager.UpdateAsync(user);

			if (result.Succeeded)
			{
				_notification.Success("Unlock User Successfully!");
				user.IsLocked = false;
			}
			else
			{
				_notification.Error("Unlock User Fail!");
			}
			return RedirectToAction("Index", "User", new { area = "Admin" });
		}

		[HttpPost]
		[Authorize]
		public IActionResult Logout()
		{
			_signInManager.SignOutAsync();
			_notification.Success("You logged out successfully!");
			return RedirectToAction("Index", "Home", new { area = "Default" });
		}

		[HttpGet("AccessDenied")]
		[Authorize]
		public IActionResult AccessDenied()
		{
			return View();
		}

		[HttpGet("EditProfile")]
		[Authorize]
		public async Task<IActionResult> EditProfile()
		{
			//var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
			//var userId = userIdClaim?.Value;

			//var user = await _userManager.FindByIdAsync(userId);

			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				_notification.Error("This User Does Not Exist!");
				return RedirectToAction("EditProfile", "User", new { area = "Admin" });
			}

			var vm = new EditProfileVM
			{
				Id = user.Id,
				Username = user.UserName,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email
			};

			return View(vm); // Trả về view với thông tin người dùng để chỉnh sửa
		}

		[HttpPost("EditProfile")]
		[Authorize]
		public async Task<IActionResult> EditProfile(EditProfileVM vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm); // Nếu dữ liệu không hợp lệ, hiển thị lại form chỉnh sửa với thông tin đã nhập
			}

			var user = await _userManager.FindByIdAsync(vm.Id);
			if (user == null)
			{
				_notification.Error("This User Does Not Exist!");
				return RedirectToAction("EditProfile", "User", new { area = "Admin" });
			}

			// Cập nhật thông tin người dùng từ form chỉnh sửa
			user.UserName = vm.Username;
			user.FirstName = vm.FirstName;
			user.LastName = vm.LastName;
			user.Email = vm.Email;

			var updateResult = await _userManager.UpdateAsync(user);

			if (updateResult.Succeeded)
			{
				_notification.Success("Profile updated successfully!");
				return RedirectToAction("EditProfile", "User", new { area = "Admin" }); // Điều hướng đến trang thông tin người dùng sau khi cập nhật thành công
			}
			else
			{
				_notification.Error("Failed to update profile. Please try again.");
				return View(vm); // Nếu cập nhật thất bại, hiển thị lại form chỉnh sửa với thông tin đã nhập
			}
		}


	}
}
