namespace Carrental.Models
{
    public interface ICarFilters
    {
        List<Car> FilterCarsByAvailability(List<Car> cars, DateTime? pickupDate, DateTime? returnDate);
        List<Car> FilterCarsForSaleOrRent(List<Car> cars, bool forSale);
        List<Car> ApplyMakeFilter(List<Car> cars, string make);
        List<Car> ApplyModelFilter(List<Car> cars, string model);
        List<Car> FilterCarsByPriceRange(List<Car> cars, int minPrice, int maxPrice);
        List<Car> ApplyTypeFilter(List<Car> cars, List<string> types);
        List<Car> ApplyColorFilter(List<Car> cars, List<string> colors);
        List<Car> ApplyAvailabilityFilter(List<Car> cars, bool availableNow);
    }
}
