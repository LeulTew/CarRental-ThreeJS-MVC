namespace Carrental.Models
{
    public class CarVersion
    {
        public int CarVersionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        // Foreign key to Car
        public int CarId { get; set; }
        public Car Car { get; set; }
    }

}
