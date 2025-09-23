namespace Carrental.Services
{
    public class AuthMessageSenderOptions
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string Email { get; set; }
        public string Password { get; set; }
        // Back-compat with existing appsettings keys
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public bool EnableSsl { get; set; } = true;
    }

}
