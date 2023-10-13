using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;

namespace BlogWebsite.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Your Name", "your-email@example.com"));
            emailMessage.To.Add(new MailboxAddress("Recipient Name", toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            var body = new TextPart("plain")
            {
                Text = message
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.example.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("your-username", "your-password");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
