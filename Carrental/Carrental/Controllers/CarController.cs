using Microsoft.AspNetCore.Mvc;
using Carrental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Google.Api;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Carrental.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Carrental.Controllers
{
    [Authorize]
    public class CarController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly CarContext _carContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICarRepository _carRepository;
        private readonly IBookingRepository _bookRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly ICarVersionRepository _carVersionRepository;
        private readonly ICarFilters _carFilters;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Carrental.Services.IEmailSender emailSender;

        public CarController(IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager, CarContext carContext, ICarRepository carRepository,
            IBookingRepository bookRepository, SignInManager<AppUser> signInManager, IPaymentRepository paymentRepository, IReviewRepository reviewRepository,
            IFavoriteRepository favoriteRepository, ICarVersionRepository carVersionRepository, ICarFilters carFilters, IUnitOfWork unitOfWork, Carrental.Services.IEmailSender emailSender)
        {
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _carContext = carContext;
            _carRepository = carRepository;
            _bookRepository = bookRepository;
            _signInManager = signInManager;
            _paymentRepository = paymentRepository;
            _reviewRepository = reviewRepository;
            _favoriteRepository = favoriteRepository;
            _carVersionRepository = carVersionRepository;
            _carFilters = carFilters;
            _unitOfWork = unitOfWork;
            this.emailSender = emailSender;
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1, string make = null, string model = null, DateTime? pickupDate = null, DateTime? returnDate = null,
                        bool showForSale = false, bool availableNow = false, bool favorites = false, string[] types = null, string[] colors = null, string sort = "default"
                        , int minPrice = 22, int maxPrice = 1000)
        {
            // Retrieve all cars from the databaseB
            var allCars = _carRepository.GetAllCars();

            // Get all distinct car makes and models
            var allCarMakes = _carRepository.GetAllCarMakes();
            var allCarModels = _carRepository.GetAllCarModels(make);

            // Filter cars based on pickup and return dates
            allCars = _carFilters.FilterCarsByAvailability(allCars, pickupDate, returnDate);

            // Filter cars based on whether they are for sale or not
            allCars = _carFilters.FilterCarsForSaleOrRent(allCars, showForSale);

            // Apply make filter
            allCars = _carFilters.ApplyMakeFilter(allCars, make);

            // Apply model filter
            allCars = _carFilters.ApplyModelFilter(allCars, model);

            // Apply type filter
            allCars = _carFilters.ApplyTypeFilter(allCars, types.ToList());

            // Apply color filter
            allCars = _carFilters.ApplyColorFilter(allCars, colors.ToList());

            // Apply availability filter
            allCars = _carFilters.ApplyAvailabilityFilter(allCars, availableNow);

            if(!showForSale)
            // Apply price range filter
                allCars = _carFilters.FilterCarsByPriceRange(allCars, minPrice, maxPrice); 

            // Apply favorites filter
            allCars = ApplyFavoritesFilter(allCars, favorites);
            var isFavoriteList = new List<bool>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            foreach (var car in allCars)
            {
                var isFavorite = _favoriteRepository.IsCarFavoritedByUser(userId, car.CarId);
                isFavoriteList.Add(isFavorite);
            }
            // Sort cars based on the selected option
            switch (sort)
            {
                case "price-low-to-high":
                    allCars = showForSale ? allCars.OrderBy(c => c.SalePrice).ToList() : allCars.OrderBy(c => c.RentPricePerHour).ToList();
                    break;
                case "price-high-to-low":
                    allCars = showForSale ? allCars.OrderByDescending(c => c.SalePrice).ToList() : allCars.OrderByDescending(c => c.RentPricePerHour).ToList();
                    break;
                case "default":
                    // Do nothing, keep the original order
                    break;
                default:
                    break;
            }

            // Calculate pagination values
            int carsPerPage = 10; // Adjust this value as needed

            // Create the view model
            var carListModel = new CarListModel
            {
                Cars = allCars,
                CarsPerPage = carsPerPage,
                CurrentPage = page,
                SelectedMake = make ?? "All",
                SelectedModel = model ?? "All",
                AllCarMakes = allCarMakes,
                AllCarModels = allCarModels,
                Types = types.Any() ? types.ToList() : new List<string>(),
                Colors = colors.Any() ? colors.ToList() : new List<string>(),
                IsAvailableNow = availableNow,
                isFav = favorites,
                ShowForSale = showForSale,
                IsFavorite = isFavoriteList,
                SortBy = sort,
                MinPrice = minPrice, 
                MaxPrice = maxPrice 
            };

            ViewBag.ShowForSale = showForSale;

            // Return the list of cars to the view
            return View(carListModel);
        }

        // Function to apply favorites filter
        private List<Car> ApplyFavoritesFilter(List<Car> cars, bool favorites)
        {
            if (favorites)
            {
                var user = _signInManager.UserManager.GetUserAsync(User).Result;
                var favoriteCars = _carRepository.GetFavoriteCars(user.Id);

                // Filter cars to include only those marked as favorites
                cars = cars.Where(car => favoriteCars.Contains(car.CarId)).ToList();
            }
            return cars;
        }
        [HttpPost]
        [Authorize]
        public IActionResult ToggleFavorite(int carId)
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the car is already in favorites of the user
            var isFavorite = _favoriteRepository.IsCarFavoritedByUser(userId, carId);

            if (!isFavorite)
            {
                var favorite = new Favorite
                {
                    CarId = carId,
                    UserId = userId
                };

                _favoriteRepository.AddFavorite(favorite);
            }
            else
            {
                // Remove the car from favorites
                var favorite = _favoriteRepository.GetFavorite(userId, carId);
                _favoriteRepository.RemoveFavorite(favorite);
            }
            // Redirect back to the original page
            return RedirectToAction("Index");
        }






        [AllowAnonymous]
        public IActionResult Search(string searchTerm, int page = 1, bool showForSale = false)
        {
            var searchResults = _carRepository.SearchCars(searchTerm);

            // Filter cars based on whether they are for sale or not
            searchResults = _carFilters.FilterCarsForSaleOrRent(searchResults, showForSale);

            int carsPerPage = 10;
            var paginatedResults = searchResults.Skip((page - 1) * carsPerPage).Take(carsPerPage).ToList();

            ViewData["showForSale"] = showForSale;
            ViewData["searchTerm"] = searchTerm;

            // Retrieve all car makes for the filter dropdown
            var allCarMakes = _carRepository.GetAllCarMakes();

            // Create the view model
            var carListModel = new CarListModel
            {
                Cars = paginatedResults,
                CarsPerPage = carsPerPage,
                CurrentPage = page,
                ShowForSale = showForSale,
                AllCarMakes = allCarMakes,
                IsAvailableNow = false,
                IsFavorite = null,
                isFav = false
            };

            // Return the view with the view model
            return View("Index", carListModel);
        }






        [Authorize(Roles = ("Admin, Provider, Manager"))]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = ("Admin, Provider, Manager"))]
        [HttpPost]
        public IActionResult Create(CarViewModel carViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Model validation failed, return the view with validation errors
                return View(carViewModel);
            }
            string uniqueFileName = "";
            string fileName = "";
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            if (carViewModel.Image != null)
            {
                uniqueFileName = Guid.NewGuid().ToString() + "_" + carViewModel.Image.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                carViewModel.Image.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            var car = new Car
            {
                Make = carViewModel.Make,
                Model = carViewModel.Model,
                Year = carViewModel.Year,
                Description = carViewModel.Description,
                Seats = carViewModel.Seats,
                Color = carViewModel.Color,
                Condition = carViewModel.Condition,
                ImageUrl = "/images/" + uniqueFileName,
                RentPricePerHour = carViewModel.RentPricePerHour,
                SalePrice = carViewModel.SalePrice,
                CarType = carViewModel.CarType,
                IsForSale = carViewModel.IsForSale,
                Specifications = carViewModel.Specifications
            };

            if (carViewModel.IsForSale)
            {
                // Add default car version
                var defaultVersion = new CarVersion
                {
                    Name = "Default Version",
                    Description = "Default Description",
                    Price = carViewModel.SalePrice ?? 0, // Use the sale price as the default version price if available
                    Car = car
                };
                car.Versions.Add(defaultVersion);
            }

            car.Images = new List<CarImage>();
            foreach (IFormFile photo in carViewModel.Images ?? new List<IFormFile>())
            {
                if (photo == null || photo.Length == 0)
                {
                    // Handle case where no files were selected
                    continue; // Skip to the next iteration
                }

                // Generate a unique filename for each image
                var galleryFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                var galleryUniqueFileName = Path.Combine(uploadFolder, galleryFileName);
                photo.CopyTo(new FileStream(galleryUniqueFileName, FileMode.Create));

                var carGallary = new CarImage();
                carGallary.ImageUrl = "/Images/" + galleryFileName;
                car.Images.Add(carGallary);
            }

            _carRepository.AddCar(car);

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = ("Admin, Provider, Manager"))]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var car = _carRepository.GetCarById(id);

            if (car == null)
            {
                return NotFound();
            }

            var carViewModel = new CarViewModel
            {
                CarId = car.CarId,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                Description = car.Description,
                Seats = car.Seats,
                Color = car.Color,
                Condition = car.Condition,
                RentPricePerHour = car.RentPricePerHour,
                SalePrice = car.SalePrice,
                IsForSale = car.IsForSale,
                CarType = car.CarType,
                Specifications = car.Specifications,
                Images = new List<IFormFile>()
            };

            // Load the main image if it exists
            if (!string.IsNullOrEmpty(car.ImageUrl))
            {
                try
                {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, car.ImageUrl.TrimStart('/'));
                byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
                carViewModel.Image = new FormFile(new MemoryStream(imageData), 0, imageData.Length, "Image", car.ImageUrl);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            // Load additional images if they exist
            foreach (CarImage image in car.Images)
            {
                if (!string.IsNullOrEmpty(image.ImageUrl))
                {
                    try
                    {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                    byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
                    var imageFile = new FormFile(new MemoryStream(imageData), 0, imageData.Length, "Images", image.ImageUrl);
                    carViewModel.Images.Add(imageFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return View(carViewModel);
        }


        [Authorize(Roles = ("Admin, Provider, Manager"))]
        [HttpPost]
        public IActionResult Edit(int id, CarViewModel carViewModel)
        {
            string uniqueCFileName = "";
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            Car existingCar = _carRepository.GetCarById(id);
            string coverFileName = "";
            if (carViewModel.Image != null)
            {
                coverFileName = carViewModel.Image.FileName;
            }

            if (ModelState.IsValid)
            {


                if (carViewModel.Image != null && carViewModel.Image.Length > 0)
                {
                    coverFileName = Guid.NewGuid().ToString() + "_" + carViewModel.Image.FileName;
                    uniqueCFileName = Path.Combine(uploadFolder, coverFileName);
                    using (var fileStream = new FileStream(uniqueCFileName, FileMode.Create))
                    {
                        carViewModel.Image.CopyTo(fileStream);
                        fileStream.Close(); // Close the file stream after copying
                    }
                }

                var car = new Car
                {
                    CarId = id,
                    Make = carViewModel.Make,
                    Model = carViewModel.Model,
                    Year = carViewModel.Year,
                    Description = carViewModel.Description,
                    Seats = carViewModel.Seats,
                    Color = carViewModel.Color,
                    Condition = carViewModel.Condition,
                    ImageUrl = string.IsNullOrEmpty(coverFileName) ? existingCar.ImageUrl : "/images/" + coverFileName,
                    RentPricePerHour = carViewModel.RentPricePerHour,
                    SalePrice = carViewModel.SalePrice,
                    CarType = carViewModel.CarType,
                    IsForSale = carViewModel.IsForSale,
                    Specifications = carViewModel.Specifications
                };

                car.Images = new List<CarImage>();

                if (carViewModel.Images != null && carViewModel.Images.Count > 0)
                {
                    foreach (var image in carViewModel.Images)
                    {
                        string fileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                        string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            image.CopyTo(fileStream);
                            fileStream.Close();
                        }

                        // Add the new image to the car's image gallery
                        var imageGallary = new CarImage
                        {
                            ImageUrl = "/images/" + fileName,
                            CarId = id // Assuming CarId is used in the ProductGallary model to associate the image with the car
                        };

                        // Add the image to the car's image gallery
                        car.Images.Add(imageGallary);
                    }
                }

                var updatedCarResult = _carRepository.UpdateCar(car);

                if (updatedCarResult != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Handle failure to update
                    return RedirectToAction("Error");
                }
            }

            // If model state is not valid, return to the edit view with validation errors
            return View(carViewModel);
        }



        [Authorize(Roles = ("Admin, Provider, Manager"))]
        [HttpPost]
        public IActionResult Delete(int? carId)
        {
            if (carId == null)
            {
                return BadRequest();
            }

            var car = _carRepository.GetCarById(carId.Value);
            if (car == null)
            {
                return NotFound();
            }

            bool deleted = _carRepository.DeleteCar(carId.Value);
            if (deleted)
                return RedirectToAction(nameof(Index));
            else
                return RedirectToAction("Error");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Details(int id)
        {
            // Retrieve car details from the database based on id
            var car = _carRepository.GetCarById(id);

            // Retrieve all bookings for the car
            var bookings = _bookRepository.GetBookingsByCarId(car.CarId);

            // Check if the car is for sale before retrieving versions
            List<CarVersion> versions = null;
            if (car.IsForSale)
            {
                // Retrieve versions for the car
                versions = _carVersionRepository.GetVersionsByCarId(car.CarId);
            }

            // Retrieve reviews for the car
            var reviews = _reviewRepository.GetReviewsByCarId(car.CarId);

            // Retrieve favorites for the car
            var favorites = _favoriteRepository.GetFavoritesByCarId(car.CarId);

            // Create and populate the view model
            var viewModel = new CarDetailsViewModel
            {
                Car = car,
                Bookings = bookings,
                Reviews = reviews,
                Favorites = favorites,
                Versions = versions
            };

            return View(viewModel);
        }




        [Authorize]
        [HttpPost]
        public IActionResult Book(int carId, DateTime rentalStartDate, DateTime rentalEndDate, decimal totalAmount)
        {

            // Retrieve the current user
            var user = _signInManager.UserManager.GetUserAsync(User).Result;

            // Check if the user is authenticated
            if (user == null)
            {
                // Redirect to the login page if the user is not authenticated
                return RedirectToAction("Login", "Account");
            }

            // Calculate total amount based on rental duration and car's rent price per hour
            var durationInHours = (rentalEndDate - rentalStartDate).TotalHours;

            // Create a new booking object
            var booking = new Booking
            {
                CarId = carId,
                UserId = user.Id, // Use the user's ID as it shouldn't be null if authenticated
                RentalStartDate = rentalStartDate,
                RentalEndDate = rentalEndDate,
                TotalAmount = totalAmount,
                IsCompleted = false // Initially, the booking is not completed until payment is successful
            };

            // Add the booking to the database
            _bookRepository.AddBooking(booking);

            // Redirect to the payment page with the booking ID
            return RedirectToAction("ProcessPayment", booking);
        }

        [HttpPost]
        public IActionResult BuyNow(int carId, decimal totalAmount)
        {
            // Retrieve the current user
            var user = _signInManager.UserManager.GetUserAsync(User).Result;

            // Check if the user is authenticated
            if (user == null)
            {
                // Redirect to the login page if the user is not authenticated
                return RedirectToAction("Login", "Account");
            }
            // Create a booking object with the required parameters
            var booking = new Booking
            {
                CarId = carId,
                UserId = user.Id,
                TotalAmount = totalAmount,
                // Set other attributes to null or default values
                RentalStartDate = default,
                RentalEndDate = default,
                IsCompleted = default,
                PaymentTransactionId = null
            };

            // Call the ProcessPayment method
            return RedirectToAction("ProcessPayment", booking);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ProcessPayment(Booking booking)
        {
            // Generate a unique transaction ID
            string transactionId = Guid.NewGuid().ToString(); // Example: Generate a GUID as the transaction ID

            // Update the booking with the transaction ID
            booking.PaymentTransactionId = transactionId;
            _bookRepository.UpdateBooking(booking);
            var user = await _userManager.FindByIdAsync(booking.UserId);
            var model = new PaymentViewModel
            {
                BookingId = booking.BookingId,
                TotalAmount = booking.TotalAmount,
                UserName = user.UserName,
                TransactionId = booking.PaymentTransactionId
            };
            return View(model);
        }


        [Authorize, HttpPost]
        public async Task<IActionResult> PayUsingPayPal(int BookingId, decimal TotalAmount)
        {
            try
            {
                // Generate PayPal payment details
                decimal exchangeRate = 0.0205m;
                decimal amount = TotalAmount * exchangeRate;
                string returnUrl = "https://localhost:7022/Car/Success";
                string cancelUrl = "https://localhost:7022/Car/Cancel" + "?bookingId=" +BookingId;
                // Create a PayPal order
                var createdPayment = await _unitOfWork.PaypalServices.CreateOrderAsync(amount, returnUrl, cancelUrl);
                // Get the PayPal approval URL
                string approvalUrl = createdPayment.links.FirstOrDefault(x => x.rel.ToLower() == "approval_url")?.href;

                // Pass necessary parameters to the Success action
                TempData["TotalAmount"] = TotalAmount.ToString();
                TempData["BookingId"] = BookingId.ToString();

                // Redirect User to PayPal approval URL
                if (!string.IsNullOrEmpty(approvalUrl))
                {
                    return Redirect(approvalUrl);
                }
                else
                {
                    TempData["Error"] = "Failed to initiate PayPal payment";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
        {
            // Validate payment details (e.g., card number, expiry date, CVV)
            if (!ModelState.IsValid)
            {
                // If payment details are not valid, return to the payment page with validation errors
                return View("ProcessPayment", model);
            }

            // Update the booking to include the payment transaction ID
            var booking = _bookRepository.GetBookingById(model.BookingId);
            booking.PaymentTransactionId = model.TransactionId;
            _bookRepository.UpdateBooking(booking);

            // Update the car's booking status
            var car = _carRepository.GetCarById(booking.CarId);
            if (booking.RentalEndDate < DateTime.Today)
            {
                // If the rental end date has passed, mark the booking as completed
                booking.IsCompleted = true;
                // Update Car.IsBooked to false
                car.IsBooked = false;
            }
            else
            {
                // If the rental end date is in the future, mark the car as booked
                car.IsBooked = true;
            }
            // Update the Car entity
            _carRepository.UpdateCar(car);
            // Add payment to the database
            _paymentRepository.AddPayment(new Payment
            {
                BookingId = model.BookingId,
                Amount = model.TotalAmount,
                PaymentDate = DateTime.Now,
                TransactionId = model.TransactionId,
            });

            // Redirect to a confirmation page if payment is successful
            return RedirectToAction("Confirmation", new { bookingId = model.BookingId });
        }

        [Authorize]
        public async Task<IActionResult> Confirmation(int bookingId)
        {
            var booking = _bookRepository.GetBookingById(bookingId);
            bool status = true;

            if (booking == null || string.IsNullOrEmpty(booking.PaymentTransactionId))
            {
                _bookRepository.DeleteBooking(booking);
                _paymentRepository.DeletePayment(booking.PaymentTransactionId);
                status = false;
            }

            var car = _carRepository.GetCarById(booking.CarId);

            if (car == null)
            {
                status = false;
            }

            bool isForSale = car.IsForSale;
            var user = await _userManager.FindByIdAsync(booking.UserId);
            // Additional information
            var viewModel = new BookingConfirmationViewModel
            {
                IsForSale = isForSale,
                PayeeName = user.UserName,
                CarMake = car.Make,
                CarModel = car.Model,
                PaymentAmount = booking.TotalAmount,
                PaymentDate = DateTime.Now,
                TransactionId = booking.PaymentTransactionId,
                Status = status ? "Completed" : "Failed"
            };

            return RedirectToAction("BookingConfirmation", "Car", viewModel);
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult> BookingConfirmation(BookingConfirmationViewModel viewModel)
        {
            if (viewModel.IsForSale)
            {
                viewModel.PickUpDate = DateTime.Now.AddDays(7);
            }
            else
            {
                viewModel.PickUpLocation = "Megenagia Parking";
                viewModel.PickUpTime = "10:00 AM";
            }
            // Send the email
            var user = await _signInManager.UserManager.GetUserAsync(User);
            if (user == null)
            {
                // Redirect to the login page if the user is not authenticated
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await emailSender.SendEmailAsync(user.Email, "Booking Confirmation", GetEmailContent(viewModel));
            }
            catch (Exception ex)
            {
                // Handle the exception
                // Log or show error message to the user
            }

            return View(viewModel);
        }


private string GetEmailContent(BookingConfirmationViewModel viewModel)
    {
        string emailContent = string.Empty;

            // Read the email content from the HTML file
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Home", "EmailConfirm.cshtml");
        if (System.IO.File.Exists(filePath)) // Fully qualify the File class
        {
            emailContent = System.IO.File.ReadAllText(filePath); // Fully qualify the File class

            // Replace placeholders with actual data
            emailContent = emailContent.Replace("{{CarMake}}", viewModel.CarMake);
            emailContent = emailContent.Replace("{{CarModel}}", viewModel.CarModel);
            emailContent = emailContent.Replace("{{PaymentAmount}}", viewModel.PaymentAmount.ToString());
            emailContent = emailContent.Replace("{{PaymentDate}}", viewModel.PaymentDate.ToString());
            emailContent = emailContent.Replace("{{TransactionId}}", viewModel.TransactionId);
            emailContent = emailContent.Replace("{{PickUpLocation}}", viewModel.PickUpLocation);
            emailContent = emailContent.Replace("{{PickUpTime}}", viewModel.PickUpTime);
        }
        else
        {
            // Handle the case where the HTML file does not exist
            emailContent = "Booking confirmation email template not found.";
        }

        return emailContent;
    }


    [HttpGet]
        public async Task<IActionResult> Success(string paymentId, string token, string PayerID)
        {
            try
            {
                // Retrieve necessary parameters from TempData
                decimal totalAmount = decimal.Parse(TempData["TotalAmount"].ToString());
                int bookingId = int.Parse(TempData["BookingId"].ToString());

                // Handle successful PayPal payment here
                // Update the database with PayPal payment details
                var booking = _bookRepository.GetBookingById(bookingId);
                booking.PaymentTransactionId = paymentId;
                _bookRepository.UpdateBooking(booking);

                var car = _carRepository.GetCarById(booking.CarId);
                if (booking.RentalEndDate < DateTime.Today)
                {
                    booking.IsCompleted = true;
                    car.IsBooked = false;
                }
                else
                {
                    car.IsBooked = true;
                }

                _carRepository.UpdateCar(car);

                _paymentRepository.AddPayment(new Payment
                {
                    BookingId = bookingId,
                    Amount = totalAmount,
                    PaymentDate = DateTime.Now,
                    TransactionId = paymentId
                });

                // Redirect to a confirmation page if payment is successful
                return RedirectToAction("Confirmation", new { bookingId = bookingId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Cancel(int bookingId)
        {
            try
            {
                var booking = _bookRepository.GetBookingById(bookingId);
                if (booking != null)
                {
                    // Delete the booking
                    _bookRepository.DeleteBooking(booking);

                    // Delete associated payment, if any
                    if (!string.IsNullOrEmpty(booking.PaymentTransactionId))
                    {
                        _paymentRepository.DeletePayment(booking.PaymentTransactionId);
                    }

                    // Redirect to a confirmation page or home page
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction(nameof(Index)); 
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index)); 
            }
        }

        




        [HttpPost]
        [Authorize]
        public IActionResult AddReview(int carId, string comment, int rating)
        {
            // Get the current user
            var user = _signInManager.UserManager.GetUserAsync(User).Result;

            // Create a new review object
            var review = new Review
            {
                CarId = carId,
                UserId = user.Id,
                Comment = comment,
                Rating = rating,
                ReviewDate = DateTime.Now, // Assuming you want to use the current date and time
                IsApproved = false // Set the initial value for IsApproved
            };

            // Add the review to the database using your repository
            _reviewRepository.AddReview(review);

            // Redirect back to the details page
            return RedirectToAction("Details", new { id = carId });
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToFavorites(int carId)
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the car is already in favorites of the user
            var isFavorite = _favoriteRepository.GetFavoritesByCarIdAndUserId(carId, userId).Any();

            if (!isFavorite)
            {
                var favorite = new Favorite
                {
                    CarId = carId,
                    UserId = userId
                };

                _favoriteRepository.AddFavorite(favorite);
            }

            return RedirectToAction("Details", new { id = carId });
        }


        [HttpPost]
        [Authorize]
        public IActionResult RemoveFromFavorites(int carId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var favorites = _favoriteRepository.GetFavoritesByCarIdAndUserId(carId, userId);

            if (favorites != null && favorites.Any())
            {
                foreach (var favorite in favorites)
                {
                    _favoriteRepository.RemoveFavorite(favorite);
                }
            }

            return RedirectToAction("Details", new { id = carId });
        }

        // Assuming you have an endpoint to retrieve user favorites
        [HttpGet("Car/GetUserFavorites")]
        public async Task<IActionResult> GetUserFavorites()
        {
            try
            {
                // Get the current user
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser != null)
                {
                    // Get the current user ID
                    string currentUserId = currentUser.Id;

                    // Query the database to retrieve the list of favorite cars for the current user
                    var userFavorites = _favoriteRepository.GetFavoriteCarsByUserId(currentUserId);

                    if (userFavorites != null && userFavorites.Any())
                        return Ok(userFavorites);
                    else
                        return BadRequest();
                }
                else
                {
                    // Handle case where current user is not found
                    return NotFound("Current user not found.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                Console.Error.WriteLine($"Error retrieving user favorites: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        public IActionResult List()
        {
            var cars = _carContext.Cars.ToList();
            return View(cars);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public IActionResult Update(Car car)
        {
            if (ModelState.IsValid)
            {
                _carContext.Cars.Update(car);
                _carContext.SaveChanges();
                return RedirectToAction("List");
            }
            return View(car);
        }
    }

}

