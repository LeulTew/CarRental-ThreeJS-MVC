namespace Carrental.Models
{
    public interface ICarRepository
    {
        Car AddCar(Car car);
        List<Car> GetAllCars();
        List<Car> SearchCars(string searchTerm);
        Car GetCarById(int carId);
        Car UpdateCar(Car updatedCar);
        bool DeleteCar(int carId);
        List<string> GetAllCarMakes();
        List<string> GetAllCarModels(string make);
        List<int> GetFavoriteCars(string userId);
    }

}
