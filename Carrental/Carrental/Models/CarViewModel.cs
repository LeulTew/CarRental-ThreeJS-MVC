using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Carrental.Models
{
    public class CarViewModel
    {
        public int CarId { get; set; }

        [Display(Name = "Make")]
        [Required(ErrorMessage = "Make is required")]
        public string Make { get; set; }

        [Display(Name = "Model")]
        [Required(ErrorMessage = "Model is required")]
        public string Model { get; set; }

        [Display(Name = "Year")]
        [Required(ErrorMessage = "Year is required")]
        public int Year { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Seats")]
        [Required(ErrorMessage = "Number of seats is required")]
        public int Seats { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }

        [Display(Name = "Condition")]
        public string Condition { get; set; }

        [Display(Name = "Main Image")]
        public IFormFile Image { get; set; }

        [Display(Name = "Additional Images")]
        public List<IFormFile>? Images { get; set; }

        [Display(Name = "Rent Price Per Hour")]
        [Required(ErrorMessage = "Rent price per hour is required")]
        public decimal RentPricePerHour { get; set; }

        [Display(Name = "Sale Price")]
        public decimal? SalePrice { get; set; }
        [Required(ErrorMessage = "Car type is required")]
        [RegularExpression("(Sedan|Wagon|Van|Minivan|Pickup|Coupe|Hatchback|SUV)", ErrorMessage = "Invalid car type")]
        public string CarType { get; set; }

        [Display(Name = "Is For Sale")]
        public bool IsForSale { get; set; }

        [Display(Name = "Specifications")]
        public string? Specifications { get; set; }


    }
}
