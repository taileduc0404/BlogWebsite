namespace BlogWebsite.Models
{
    public class SmtpSettings
    {
        public int Id { get; set; }
        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
    }
}
