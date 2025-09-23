using Microsoft.AspNetCore.Identity;

namespace Carrental.Models
{
    public class CarFilters : ICarFilters
    {
        private readonly IBookingRepository _bookRepository;

        public CarFilters(IBookingRepository bookingRepository)
        {
            _bookRepository = bookingRepository;
        }

        // Function to filter cars based on pickup and return dates
        public List<Car> FilterCarsByAvailability(List<Car> cars, DateTime? pickupDate, DateTime? returnDate)
        {
            if (pickupDate != null && returnDate != null)
            {
                cars = cars.Where(car =>
                {
                    // Check if the car is available for the selected pickup and return dates
                    return !_bookRepository.IsCarBookedForDateRange(car.CarId, pickupDate.Value, returnDate.Value);
                }).ToList();
            }
            return cars;
        }

        // Function to filter cars based on whether they are for sale or for rent
        public List<Car> FilterCarsForSaleOrRent(List<Car> cars, bool forSale)
        {
            if (forSale)
            {
                return cars.Where(car => car.IsForSale).ToList();
            }
            else
            {
                return cars.Where(car => !car.IsForSale).ToList();
            }
        }

        // Function to apply make filter
        public List<Car> ApplyMakeFilter(List<Car> cars, string make)
        {
            if (!string.IsNullOrEmpty(make) && make != "All")
            {
                cars = cars.Where(car => car.Make == make).ToList();
            }
            return cars;
        }

        // Function to apply model filter
        public List<Car> ApplyModelFilter(List<Car> cars, string model)
        {
            if (!string.IsNullOrEmpty(model) && model != "All")
            {
                cars = cars.Where(car => car.Model == model).ToList();
            }
            return cars;
        }
        // Function to filter cars based on selected types
        public List<Car> ApplyTypeFilter(List<Car> cars, List<string> types)
        {
            if (types != null && types.Any())
            {
                // Convert types to lowercase for comparison
                var lowerCaseTypes = types.Select(t => t.ToLower()).ToList();

                // Filter cars based on selected types (converting to lowercase for comparison)
                return cars.Where(car => lowerCaseTypes.Contains(car.CarType.ToLower())).ToList();
            }
            return cars;
        }
        public List<Car> ApplyColorFilter(List<Car> cars, List<string> colors)
        {
            if (colors != null && colors.Any())
            {
                // Convert colors to lowercase for comparison
                var lowerCaseColors = colors.Select(c => c.ToLower()).ToList();

                // Check if 'other' color is selected
                if (lowerCaseColors.Contains("other"))
                {
                    // Filter cars excluding the ones with specified colors
                    return cars.Where(car => !lowerCaseColors.Any(color => car.Color.ToLower() == color)).ToList();
                }
                else
                {
                    // Filter cars based on selected colors (converting to lowercase for comparison)
                    return cars.Where(car => lowerCaseColors.Contains(car.Color.ToLower())).ToList();
                }
            }
            return cars;
        }
        // Function to apply availability filter
        public List<Car> ApplyAvailabilityFilter(List<Car> cars, bool availableNow)
        {
            if (availableNow)
            {
                // Filter cars to include only those available now
                return cars.Where(car => !car.IsBooked).ToList();
            }
            return cars;
        }

        public List<Car> FilterCarsByPriceRange(List<Car> cars, int minPrice, int maxPrice)
        {
            return cars.Where(car => car.RentPricePerHour >= minPrice && car.RentPricePerHour <= maxPrice).ToList();
        }

    }
}
