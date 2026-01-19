using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using EventEaseApp.Validation;

namespace EventEaseApp.Models
{
    public class UserRegistration : IValidatableObject
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name must be less than 50 characters")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name must be less than 50 characters")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email must be less than 100 characters")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-18);

        public string Company { get; set; } = "";
        public string JobTitle { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = "";

        [MustBeTrue(ErrorMessage = "You must accept the Terms and Conditions to continue")]
        public bool AcceptTerms { get; set; }

        public bool ReceiveNotifications { get; set; } = true;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Validate date of birth without RangeAttribute to avoid trimming warnings.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var min = new DateTime(1900, 1, 1);
            var max = new DateTime(2010, 12, 31);
            if (DateOfBirth < min || DateOfBirth > max)
            {
                yield return new ValidationResult(
                    "Please enter a valid date of birth",
                    new[] { nameof(DateOfBirth) });
            }
        }
    }

    public class EventRegistration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = "";
        public int EventId { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Registered"; // Registered, CheckedIn, NoShow
        public string Notes { get; set; } = "";
    }

    public class UserSession
    {
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public DateTime LoginTime { get; set; } = DateTime.Now;
        public DateTime LastActivity { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public Dictionary<string, string> SessionData { get; set; } = new();
    }

    public class UserLogin
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = "";
    }

    public class UserCredential
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public string Company { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    }