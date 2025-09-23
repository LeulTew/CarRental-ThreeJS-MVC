namespace Carrental.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int CarId { get; set; }
        public string UserId { get; set; }
        public DateTime RentalStartDate { get; set; }
        public DateTime RentalEndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsCompleted { get; set; }

        // Additional properties for payment integration
        public string? PaymentTransactionId { get; set; }
    }
}
