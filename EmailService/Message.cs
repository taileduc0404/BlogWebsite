using Microsoft.AspNetCore.Http;
using MimeKit;


namespace EmailService
{
	public class Message
	{
		private readonly EmailConfiguration _emailConfig;
		public List<MailboxAddress> To { get; set; }
		public string Subject { get; set; }
		public string Content { get; set; }

		public IFormFileCollection Attachments { get; set; }

		public Message(EmailConfiguration emailConfig, IEnumerable<string> to, string subject, string content, IFormFileCollection attachments)
		{
			_emailConfig = emailConfig;
			To = new List<MailboxAddress>();
			To.AddRange(to.Select(x => new MailboxAddress(_emailConfig.FromName, x)));
			Subject = subject;
			Content = content;
			Attachments = attachments;
		}
	}
}