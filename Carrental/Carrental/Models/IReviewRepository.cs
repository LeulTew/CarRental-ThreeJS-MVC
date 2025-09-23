namespace Carrental.Models
{
    public interface IReviewRepository
    {
        List<Review> GetReviewsByCarId(int carId);
        void AddReview(Review review);
    }
}
