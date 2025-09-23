using Carrental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Carrental.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IPasswordHasher<AppUser> passwordHasher;
        private readonly SignInManager<AppUser> signInManager;

        public AdminController(UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.passwordHasher = passwordHasher;
            this.signInManager = signInManager;
        }

        [Authorize(Roles = ("Admin, Manager"))]
        public IActionResult Index()
        {
            return View(userManager.Users);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new LoginSignupViewModel();
            viewModel.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            viewModel.User = new User(); 
            return View("~/Views/home/LoginSignup.cshtml", viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(LoginSignupViewModel viewModel)
        {
            // Clear validation errors for the Login model
            ModelState.Remove("Login.Email");
            ModelState.Remove("Login.Password");
            if (ModelState.IsValid)
            {
                // Create the user
                AppUser appUser = new AppUser
                {
                    UserName = viewModel.User.Username,
                    Email = viewModel.User.Email,
                    FullName = viewModel.User.FullName,
                    Phone = viewModel.User.Phone
                };

                IdentityResult result = await userManager.CreateAsync(appUser, viewModel.User.Password);
                if (result.Succeeded)
                {
                    // Add the user to the "Customer" role
                    await userManager.AddToRoleAsync(appUser, "Customer");
                    // Automatically log in the new user
                    await signInManager.SignInAsync(appUser, isPersistent: false);
                    return RedirectToAction("Index", "Car");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            // Log ModelState errors
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key].Errors)
                {
                    Console.WriteLine($"ModelState Error for {key}: {error.ErrorMessage}");
                }
            }

            // If the sign-up fails, return the view with the validation errors
            viewModel.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View("~/Views/home/LoginSignup.cshtml", viewModel);
        }




        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Check if the user is an admin
                if (!User.IsInRole("Admin"))
                {
                    // If not admin, ensure the user can only update their own account
                    var currentUser = await userManager.GetUserAsync(User);
                    if (currentUser.Id != id)
                    {
                        // If the user is not the owner of the account to be updated, deny access
                        return Forbid();
                    }
                }

                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(string Id, string FullName, string Email, string Phone, string Password)
        {
            // Perform the same authorization check as in the GET method
            if (!User.IsInRole("Admin"))
            {
                var currentUser = await userManager.GetUserAsync(User);
                if (currentUser.Id != Id)
                {
                    return Forbid();
                }
            }

            AppUser user = await userManager.FindByIdAsync(Id);
            if (user != null)
            {
                // Fetch the username from the user object
                string username = user.UserName;
                if (!string.IsNullOrEmpty(FullName))
                    user.FullName = FullName;
                else
                    ModelState.AddModelError("", "Full name cannot be empty");

                if (!string.IsNullOrEmpty(Email))
                    user.Email = Email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(Phone))
                    user.Phone = Phone;
                else
                    ModelState.AddModelError("", "Phone number cannot be empty");

                if (!string.IsNullOrEmpty(Password))
                    user.PasswordHash = passwordHasher.HashPassword(user, Password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(FullName) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Phone) && !string.IsNullOrEmpty(Password))
                {
                    // Set the UserName property before updating
                    user.UserName = username;
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        // Check if the request is coming from the Settings page
                        var refererUrl = Request.Headers["Referer"].ToString();
                        if (!string.IsNullOrEmpty(refererUrl) && refererUrl.Contains("Account/Settings"))
                        {
                            // Redirect to the Settings page after updating the user
                            return RedirectToAction("Settings", "Account");
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError("", error.Description);

                        // Check if the request is coming from the Settings page
                        var refererUrl = Request.Headers["Referer"].ToString();
                        if (!string.IsNullOrEmpty(refererUrl) && refererUrl.Contains("Account/Settings"))
                        {
                            // Redirect to the Settings page with errors
                            return RedirectToAction("Settings", "Account");
                        }
                        else
                        {
                            // Redirect to another appropriate page with errors
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");

            return View(user);
        }



        [Authorize]
        public async Task<IActionResult> Delete(string Id)
        {
            // Check if the user is an admin
            if (!User.IsInRole("Admin"))
            {
                // If not admin, ensure the user can only delete their own account
                var currentUser = await userManager.GetUserAsync(User);
                if (currentUser.Id != Id)
                {
                    // If the user is not the owner of the account to be deleted, deny access
                    return Forbid();
                }
            }

            if (string.IsNullOrEmpty(Id))
            {
                ModelState.AddModelError("", "User ID cannot be empty");
                return RedirectToAction("Index");
            }

            AppUser user = await userManager.FindByIdAsync(Id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }

            // If deletion is unsuccessful or user not found, redirect to 'Index'
            return RedirectToAction("Index");
        }





    }
}
