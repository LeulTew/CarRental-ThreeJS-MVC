using Carrental.Models;
using System;
using System.Collections.Generic;

namespace Carrental.Models
{
    public class CarDetailsViewModel
    {
        public Car? Car { get; set; }
        public List<Booking>? Bookings { get; set; }
        public Payment? Payment { get; set; }
        public List<Review>? Reviews { get; set; }
        public List<Favorite>? Favorites { get; set; }
        public bool IsFavorite { get; set; }

        // Only include Versions if the car is for sale
        public List<CarVersion>? Versions { get; set; }
    }

}
