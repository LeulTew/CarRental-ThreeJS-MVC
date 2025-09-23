namespace Carrental.Models
{
    public class CarListModel
    {
        public List<Car> Cars { get; set; }
        public int CarsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string? SelectedMake { get; set; } 
        public string? SelectedModel { get; set; }

        public int PageCount()
        {
            if (CarsPerPage <= 0)
            {
                return 0; // or throw an exception or handle it differently based on your requirements
            }

            int count = Cars.Count();
            int pageCount = count / CarsPerPage;
            if (count % CarsPerPage != 0)
            {
                pageCount++;
            }
            return pageCount;
        }


        public List<Car> PaginatedCars()
        {
            int startIndex = (CurrentPage - 1) * CarsPerPage;
            return Cars.Skip(startIndex).Take(CarsPerPage).ToList();
        }


        // Add AllCarMakes and AllCarModels properties
        public List<string>? AllCarMakes { get; set; }
        public List<string>? AllCarModels { get; set; }
        public List<string>? Types { get; set; }
        public List<string>? Colors { get; set; }
        public bool? IsAvailableNow;
        public bool? isFav;
        public bool? ShowForSale;
        public List<bool> IsFavorite { get; set; }
        public string SortBy { get; set; }
        public int? MinPrice, MaxPrice;
    }

}
