using BlogWebsite.EmailServices;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Areas.Admin.Controllers
{
    public class ForgotPasswordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly IEmailService _emailService;

        public ForgotPasswordController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // Kiểm tra xem email có tồn tại trong hệ thống hay không

            // Nếu email tồn tại, tạo token reset password
            string resetToken = Guid.NewGuid().ToString();

            // Gửi email với token reset password
            string callbackUrl = Url.Action("ResetPassword", "Account", new { token = resetToken }, Request.Scheme)!;
            string message = $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>";

            await _emailService.SendEmailAsync(email, "Reset your password", message);

            // Redirect hoặc hiển thị thông báo thành công
            return View();
        }
    }
}
