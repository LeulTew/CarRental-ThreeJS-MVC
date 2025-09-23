using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Google;

namespace Carrental.Models
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly CarContext _context;

        public ReviewRepository(CarContext context)
        {
            _context = context;
        }

        public List<Review> GetReviewsByCarId(int carId)
        {
            return _context.Reviews.Include(r => r.User).Where(r => r.CarId == carId).ToList();
        }

        public void AddReview(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }
    }

}
