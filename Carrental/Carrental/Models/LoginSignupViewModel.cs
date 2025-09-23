using Microsoft.AspNetCore.Authentication;

namespace Carrental.Models
{
    public class LoginSignupViewModel
    {
        public User? User { get; set; }
        public Login? Login { get; set; } = new Login(); 
        public IList<AuthenticationScheme>? ExternalLogins { get; set; } = new List<AuthenticationScheme>();
    }

}
