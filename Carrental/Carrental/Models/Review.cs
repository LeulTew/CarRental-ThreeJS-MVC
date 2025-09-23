namespace Carrental.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int CarId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; } // New property for review date

        // Additional properties for moderation
        public bool IsApproved { get; set; }
        public AppUser User { get; set; }
    }
}
