using System.Collections.Generic;
using Carrental.Models;

namespace Carrental.Models
{
    public class CarAndFavoritesViewModel
    {
        public List<Car>? AllCars { get; set; }
        public List<Car>? UserFavorites { get; set; }
    }
}
