using System;
using System.Collections.Generic;

namespace Carrental.Models
{
    public class Favorite
    {
        public string UserId { get; set; }
        public int CarId { get; set; }
        public DateTime DateAdded { get; set; } // Optional: To track when the car was added to favorites

        public AppUser User { get; set; }
        public Car Car { get; set; }
    }
}
