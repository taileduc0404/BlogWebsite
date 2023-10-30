using BlogWebsite.Models;
using MimeKit;
using MailKit.Net.Smtp;

namespace BlogWebsite.EmailServices
{
    public class EmailService:IEmailService
    {

        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your Name", emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };
                
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSettings.SmtpServer, emailSettings.SmtpPort, false);
                await client.AuthenticateAsync(emailSettings.SmtpUsername, emailSettings.SmtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
