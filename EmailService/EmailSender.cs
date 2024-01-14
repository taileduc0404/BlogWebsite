using MailKit.Net.Smtp;
using MimeKit;

namespace EmailService
{
	public class EmailSender : IEmailSender
	{
		private readonly EmailConfiguration _emailConfig;

		public EmailSender(EmailConfiguration emailConfig)
		{
			_emailConfig = emailConfig;
		}

		public void SendEmail(Message message)
		{
			var emailMessage = CreateEmailMessage(message);

			Send(emailMessage);
		}

		public async Task SendEmailAsync(Message message)
		{
			var mailMessage = CreateEmailMessage(message);

			await SendAsync(mailMessage);
		}

		private MimeMessage CreateEmailMessage(Message message)
		{
			var emailMessage = new MimeMessage();
			emailMessage.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.From));
			emailMessage.To.AddRange(message.To);
			emailMessage.Subject = message.Subject;

			var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<p style='color:black;'>To reset your password, please click on the button below to set a new password.</p> <br>  <a style='display: inline-block;padding: 10px 20px;background-color: #3385b5;color: #fff;text-decoration: none;border: none;border-radius: 5px;cursor: pointer;' href='{0}' role='button'>Reset Password</a>", message.Content) };

			if (message.Attachments != null && message.Attachments.Any())
			{
				byte[] fileBytes;
				foreach (var attachment in message.Attachments)
				{
					using (var ms = new MemoryStream())
					{
						attachment.CopyTo(ms);
						fileBytes = ms.ToArray();
					}

					bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
				}
			}

			emailMessage.Body = bodyBuilder.ToMessageBody();
			return emailMessage;
		}

		private void Send(MimeMessage mailMessage)
		{
			using var client = new SmtpClient();
			{
				try
				{
					client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
					client.AuthenticationMechanisms.Remove("XOAUTH2");
					client.Authenticate(_emailConfig.UserName, "digiaqqnsxomggzr");

					client.Send(mailMessage);
				}
				catch
				{
					//log an error message or throw an exception, or both.
					throw;
				}
				finally
				{
					client.Disconnect(true);
					client.Dispose();
				}
			}
		}

		private async Task SendAsync(MimeMessage mailMessage)
		{
			using (var client = new SmtpClient())
			{
				try
				{
					await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
					client.AuthenticationMechanisms.Remove("XOAUTH2");
					await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

					await client.SendAsync(mailMessage);
				}
				catch
				{
					//log an error message or throw an exception, or both.
					throw;
				}
				finally
				{
					await client.DisconnectAsync(true);
					client.Dispose();
				}
			}
		}
	}
}