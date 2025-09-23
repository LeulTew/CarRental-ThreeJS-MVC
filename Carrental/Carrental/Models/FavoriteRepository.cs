using Google;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
namespace Carrental.Models
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly CarContext _context;

        public FavoriteRepository(CarContext context)
        {
            _context = context;
        }
        public List<Favorite> GetFavoritesByCarId(int carId)
        {
            return _context.Favorites.Where(f => f.CarId == carId).ToList();
        }
        public List<Favorite> GetFavoritesByCarIdAndUserId(int carId, string userId)
        {
            return _context.Favorites
                .Where(f => f.CarId == carId && f.UserId == userId)
                .ToList();
        }

        public void AddFavorite(Favorite favorite)
        {
            favorite.DateAdded = DateTime.Now;
            var car = _context.Cars.Find(favorite.CarId);
            if (car != null)
            {
                // Associate the Car entity with the Favorite
                favorite.Car = car;

                // Add the favorite to the context
                _context.Favorites.Add(favorite);

                // Save changes
                _context.SaveChanges();
            }
        }

        public void RemoveFavorite(Favorite favorite)
        {
            _context.Favorites.Remove(favorite);
            _context.SaveChanges();
        }

        public List<Car> GetFavoriteCarsByUserId(string userId)
        {
            return _context.Favorites
                .Include(f => f.Car) // Include the Car navigation property
                .Where(f => f.UserId == userId)
                .Select(f => f.Car) // Select the Car entity from favorites
                .ToList();
        }

        public bool IsCarFavoritedByUser(string userId, int carId)
        {
            return _context.Favorites.Any(f => f.UserId == userId && f.CarId == carId);
        }

        public Favorite? GetFavorite(string userId, int carId)
        {
            return _context.Favorites.Where(f => f.UserId == userId && f.CarId == carId).FirstOrDefault();
        }

    }
}



