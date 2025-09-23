using System.Linq;

namespace Carrental.Models
{
    public class BookingRepository : IBookingRepository
    {
        private readonly CarContext _context;

        public BookingRepository(CarContext context)
        {
            _context = context;
        }

        public void AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
        }

        public Booking GetBookingById(int bookingId)
        {
            Booking? booking = _context.Bookings.Where(b => b.BookingId == bookingId).FirstOrDefault();
            return booking;
        }

        public void UpdateBooking(Booking booking)
        {
            _context.Bookings.Update(booking);
            _context.SaveChanges();
        }

        public Booking GetBookingByCarId(int carId)
        {
            Booking? booking = _context.Bookings.Where(b => b.CarId == carId).FirstOrDefault();
            return booking;
        }
        public void DeleteBooking(Booking booking)
        {
            _context.Bookings.Remove(booking);
            _context.SaveChanges();
        }

        public List<Booking> GetBookingsByUserId(string userId)
        {
            return _context.Bookings.Where(b => b.UserId == userId).ToList();
        }

        public List<Booking> GetBookingsByCarId(int carId)
        {
            // Retrieve bookings for the specified car ID from the database
            return _context.Bookings
                .Where(b => b.CarId == carId)
                .ToList();
        }

        // Method to check if a car is booked for a given date range
        public bool IsCarBookedForDateRange(int carId, DateTime pickupDate, DateTime returnDate)
        {
            // Check if any booking overlaps with the selected date range
            return _context.Bookings.Any(b =>
                b.CarId == carId &&
                (pickupDate >= b.RentalStartDate && pickupDate <= b.RentalEndDate ||
                 returnDate >= b.RentalStartDate && returnDate <= b.RentalEndDate));
        }

    }

}
