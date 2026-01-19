using System.ComponentModel.DataAnnotations;
using EventEaseApp.Validation;

namespace EventEaseApp.Models
{
    public class RegistrationForm
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = "";

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; } = "";

        [MustBeTrue(ErrorMessage = "You must agree to the terms and conditions")]
        public bool AgreeToTerms { get; set; } = false;

        public bool ReceiveUpdates { get; set; } = false;
    }
}