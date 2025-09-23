namespace Carrental.Models
{
    public interface ICarVersionRepository
    {
        List<CarVersion> GetVersionsByCarId(int carId);
    }
}
