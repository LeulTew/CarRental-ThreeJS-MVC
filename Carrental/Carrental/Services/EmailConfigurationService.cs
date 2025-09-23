using Carrental.Models;
using Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Carrental.Services
{
    public class EmailConfigurationService : IEmailConfigurationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly CarContext _dbContext;

        public EmailConfigurationService(UserManager<AppUser> userManager, CarContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public async Task<EmailConfiguration> GetEmailConfigurationAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // User not found, handle accordingly (e.g., throw exception or return null)
                return null;
            }

            // Retrieve email configuration from the database based on user's settings
            var emailConfiguration = await _dbContext.EmailConfigurations.FirstOrDefaultAsync(e => e.UserId == userId);
            return emailConfiguration;
        }
    }
}
