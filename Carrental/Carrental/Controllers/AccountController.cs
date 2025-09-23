using Carrental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Carrental.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace CarRental.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IBookingRepository bookingRepository;
        private readonly ICarRepository carRepository;
        private readonly Carrental.Services.IEmailSender emailSender;
        private readonly EmailConfigurationService _emailConfigurationService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 IBookingRepository bookingRepository,
                                 ICarRepository carRepository, Carrental.Services.IEmailSender emailSender, EmailConfigurationService emailConfigurationService, ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.bookingRepository = bookingRepository;
            this.carRepository = carRepository;
            this.emailSender = emailSender;
            _emailConfigurationService = emailConfigurationService;
            _logger = logger;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                    new { ReturnUrl = returnUrl });

            var properties =
                signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Car");

            LoginSignupViewModel loginViewModel = new LoginSignupViewModel
            {
                Login = new Login { ReturnUrl = returnUrl },
                ExternalLogins =
                        (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginViewModel);
            }

            // Get the login information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Login", loginViewModel);
            }

            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else
            {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new AppUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            FullName = info.Principal.FindFirstValue(ClaimTypes.Name),
                            Phone = GenerateRandomEthiopianPhoneNumber() // Generate random Ethiopian phone number
                        };

                        await userManager.CreateAsync(user);
                    }
                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

                return View("Error");
            }
        }

        // Generate a random Ethiopian phone number
        private string GenerateRandomEthiopianPhoneNumber()
        {
            var random = new Random();
            var prefix = "+251"; // Ethiopian country code

            // Generate a random 9-digit number for the phone number
            var phoneNumber = random.Next(100000000, 999999999).ToString();

            return $"{prefix}{phoneNumber}";
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {

            var loginSignupViewModel = new LoginSignupViewModel
            {
                Login = new Login { ReturnUrl = returnUrl },
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()

            };

            return View("~/Views/home/LoginSignup.cshtml", loginSignupViewModel);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginSignupViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await userManager.FindByEmailAsync(vm.Login.Email);
                if (appUser != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result =
                        await signInManager.PasswordSignInAsync(appUser, vm.Login.Password, vm.Login.Remember, false);
                    if (result.Succeeded)
                        return Redirect(vm.Login.ReturnUrl ?? "/Car");
                }
                ModelState.AddModelError(nameof(vm.Login.Email), "Login Failed: Invalid Email or password");
            }
            return View("~/Views/home/LoginSignup.cshtml", vm); // Using LoginSignup.cshtml view
        }


        [AllowAnonymous]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Car");
        }

        public async Task<IActionResult> Settings()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            // Ensure that the user can only access their own settings
            var userId = userManager.GetUserId(User);
            if (user.Id != userId)
            {
                return Forbid(); // Or return Unauthorized() depending on your requirements
            }

            // Retrieve booking history
            var bookings = bookingRepository.GetBookingsByUserId(userId);

            // Map booking to car
            var bookingDetails = new List<(Booking booking, Car car)>();
            foreach (var booking in bookings)
            {
                var car = carRepository.GetCarById(booking.CarId);
                bookingDetails.Add((booking, car));
            }

            ViewBag.BookingDetails = bookingDetails;

            return View(user);
        }


        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous, HttpGet("forgot-password")]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [AllowAnonymous, HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {

                return View(model);
            }
                ModelState.Clear();
                model.EmailSent = true;
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return View(model);
            }
            var emailConfiguration = await _emailConfigurationService.GetEmailConfigurationAsync(user.Id);
            if (emailConfiguration == null)
            {
                // Email configuration not found for the user, handle accordingly
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            if (!(await userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return View(model);
            }
            // Generate the reset password token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // Generate the callback URL with the reset password token
            var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);

            try
            {
                // Send forgot password email
                await emailSender.SendForgotPasswordEmailAsync(model.Email, callbackUrl);
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                _logger.LogError(ex, "Failed to send forgot password email");
                ModelState.AddModelError(string.Empty, "Failed to send forgot password email. Please try again later.");
                return View(model);
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

    }
}


