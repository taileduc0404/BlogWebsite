using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using BlogWebsite.Models;

namespace BlogWebsite.Services
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public void SendEmail(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Duc Tai", _smtpSettings.SmtpUsername));
            message.To.Add(new MailboxAddress("My Customer", to));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.Connect(_smtpSettings.SmtpServer, _smtpSettings.SmtpPort, false);
                client.Authenticate(_smtpSettings.SmtpUsername, _smtpSettings.SmtpPassword);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }

}
