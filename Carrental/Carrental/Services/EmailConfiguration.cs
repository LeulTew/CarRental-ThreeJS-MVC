namespace Carrental.Services
{
    public class EmailConfiguration
    {
        public string SmtpPort { get; set; }
        public string SmtpServer { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }

}
