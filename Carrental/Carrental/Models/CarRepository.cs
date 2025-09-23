using Google.Api;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Carrental.Models
{
    public class CarRepository : ICarRepository
    {
        private readonly CarContext _carContext;

        public CarRepository(CarContext carContext)
        {
            _carContext = carContext;
        }

        public Car AddCar(Car car)
        {
            _carContext.Cars.Add(car);
            _carContext.SaveChanges();
            return car;
        }

        public List<Car> GetAllCars()
        {
            return _carContext.Cars.ToList();
        }

        public List<Car> SearchCars(string searchTerm)
        {
            return _carContext.Cars
                .Where(car =>
                    car.Make.Contains(searchTerm) ||
                    car.Model.Contains(searchTerm) ||
                    car.Year.ToString().Contains(searchTerm) ||
                    car.Description.Contains(searchTerm) ||
                    car.CarType.Contains(searchTerm) // Search by CarType
                )
                .ToList();
        }

        public Car GetCarById(int carId)
        {
            Car? car = _carContext.Cars.Include(i => i.Images)
                .Where(c => c.CarId == carId)
                .FirstOrDefault();
            return car;
        }

        public Car UpdateCar(Car updatedCar)
        {
            Car? existingCar = _carContext.Cars.Include(i => i.Images)
                .FirstOrDefault(c => c.CarId == updatedCar.CarId);
            if (existingCar != null)
            {
                existingCar.Make = updatedCar.Make;
                existingCar.Model = updatedCar.Model;
                existingCar.Year = updatedCar.Year;
                existingCar.Description = updatedCar.Description;
                existingCar.Seats = updatedCar.Seats;
                existingCar.Color = updatedCar.Color;
                existingCar.Condition = updatedCar.Condition;
                existingCar.ImageUrl = updatedCar.ImageUrl;
                existingCar.Images = updatedCar.Images;
                existingCar.RentPricePerHour = updatedCar.RentPricePerHour;
                existingCar.SalePrice = updatedCar.SalePrice;
                existingCar.IsForSale = updatedCar.IsForSale;
                existingCar.Specifications = updatedCar.Specifications;
                existingCar.CarType = updatedCar.CarType; // Update CarType

                _carContext.SaveChanges();
            }
            return existingCar;
        }


        public bool DeleteCar(int carId)
        {
            var carToDelete = _carContext.Cars.Find(carId);
            if (carToDelete != null)
            {
                _carContext.Cars.Remove(carToDelete);
                _carContext.SaveChanges();
                return true;
            }
            return false;
        }

        public List<string> GetAllCarMakes()
        {
            return _carContext.Cars.Select(c => c.Make).Distinct().ToList();
        }

        public List<string> GetAllCarModels(string make)
        {
            if (make == "All" || string.IsNullOrEmpty(make))
            {
                return _carContext.Cars.Select(c => c.Model).Distinct().ToList();
            }
            else
            {
                return _carContext.Cars.Where(c => c.Make == make).Select(c => c.Model).Distinct().ToList();
            }
        }

        public List<int> GetFavoriteCars(string userId)
        {
            var favoriteCarIds = _carContext.Favorites
                                        .Where(favorite => favorite.UserId == userId)
                                        .Select(favorite => favorite.CarId)
                                        .ToList();
            return favoriteCarIds;
        }

    }
}
