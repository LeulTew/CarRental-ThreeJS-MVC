namespace Carrental.Models
{
    public interface IFavoriteRepository
    {
        List<Favorite> GetFavoritesByCarId(int carId);
        List<Favorite> GetFavoritesByCarIdAndUserId(int carId, string userId);
        void AddFavorite(Favorite favorite);
        void RemoveFavorite(Favorite favorite);
        List<Car> GetFavoriteCarsByUserId(string userId);
        bool IsCarFavoritedByUser(string userId, int carId);
        Favorite GetFavorite(string userId, int carId);

    }
}
