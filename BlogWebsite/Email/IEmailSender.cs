namespace BlogWebsite.Email
{
	public interface IEmailSender
	{
		void SendEmail(Message message);
	}
}
