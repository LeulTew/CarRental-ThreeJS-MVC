namespace Carrental.Services
{
    public interface IEmailConfigurationService
    {
        Task<EmailConfiguration> GetEmailConfigurationAsync(string userId);
    }
}
