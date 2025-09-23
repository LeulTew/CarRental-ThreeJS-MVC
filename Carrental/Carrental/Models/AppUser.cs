using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Carrental.Models
{
    public class AppUser : IdentityUser
    {
        // Additional properties for user profiles
        public string FullName { get; set; }
        public string Phone { get; set; }

        // Navigation property for user favorites
        public ICollection<Favorite> Favorites { get; set; }
    }
}
