namespace Carrental.Models
{
	public class BookingConfirmationViewModel
	{
		public bool IsForSale { get; set; }
		public DateTime? PickUpDate { get; set; }
		public string? PickUpLocation { get; set; }
		public string? PickUpTime { get; set; }

        // Additional properties
        public string? PayeeName { get; set; }
        public string? CarMake { get; set; }
        public string? CarModel { get; set; }
        public decimal? PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? TransactionId { get; set; }
        public string? Status { get; set; }
    }
}
