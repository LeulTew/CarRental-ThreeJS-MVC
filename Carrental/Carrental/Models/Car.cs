using System.ComponentModel;

namespace Carrental.Models
{
    public class Car
    {
        public Car()
        {
            Versions = new List<CarVersion>();
        }
        public int CarId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public int Seats { get; set; }
        public string Color { get; set; }
        public string Condition { get; set; }
        public string ImageUrl { get; set; }
        public List<CarImage> Images { get; set; }
        public decimal RentPricePerHour { get; set; }
        public decimal? SalePrice { get; set; } // Nullable if not for sale

        public string CarType { get; set; } // Example: "SUV", "Sedan", "Truck", etc.

        // Additional properties for car status
        [DefaultValue(false)] // Example: Sets IsBooked to false by default
        public bool IsBooked { get; set; }

        [DefaultValue(false)] // Example: Sets IsNew to false by default
        public bool IsNew { get; set; }

        // Additional properties for car sales
        public bool IsForSale { get; set; }
        public string? Specifications { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        // Car versions
        public ICollection<CarVersion> Versions { get; set; }
    }
}
