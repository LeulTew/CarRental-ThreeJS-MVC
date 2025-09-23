namespace Carrental.Models
{
    public interface IBookingRepository
    {
        void AddBooking(Booking booking);
        Booking GetBookingById(int bookingId);
        void UpdateBooking(Booking booking);
        Booking GetBookingByCarId(int carId);
        void DeleteBooking(Booking booking);
        List<Booking> GetBookingsByUserId(string userId);
        List<Booking> GetBookingsByCarId(int carId);
        bool IsCarBookedForDateRange(int carId, DateTime pickupDate, DateTime returnDate);


    }

}
