namespace Carrental.Models
{
    public class CarImage
    {
        public int CarImageId { get; set; }
        public string ImageUrl { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }
    }

}
