using Microsoft.EntityFrameworkCore;

namespace Carrental.Models
{
    public class CarVersionRepository : ICarVersionRepository
    {
        private readonly CarContext _context;

        public CarVersionRepository(CarContext context)
        {
            _context = context;
        }

        public List<CarVersion> GetVersionsByCarId(int carId)
        {
            return _context.CarVersions.Where(cv => cv.CarId == carId).ToList();
        }

    }
}
