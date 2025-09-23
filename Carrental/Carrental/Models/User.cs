using System.ComponentModel.DataAnnotations;

namespace Carrental.Models
{
    public class User
    {

        // Automatically generate username based on email
        public string Username => GenerateUsername();
        [Required(ErrorMessage = "Full name is required")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+251\d{9}$|^(09|07)\d{8}$", ErrorMessage = "Please enter a valid Ethiopian phone number")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        private string GenerateUsername()
        {
            if (!string.IsNullOrEmpty(Email))
            {
                // Split email address by '@'
                string[] parts = Email.Split('@');

                // Use the first part as the username
                return parts[0];
            }
            else
            {
                // If email is null or empty, return a default username or handle this case as per your application logic
                return "DefaultUsername";
            }
        }

    }
}
