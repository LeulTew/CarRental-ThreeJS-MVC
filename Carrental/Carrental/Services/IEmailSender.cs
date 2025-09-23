using System.Threading.Tasks;
namespace Carrental.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendForgotPasswordEmailAsync(string email, string callbackUrl);
    }
}



