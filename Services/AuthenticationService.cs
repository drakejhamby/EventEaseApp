using EventEaseApp.Models;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace EventEaseApp.Services
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterUserAsync(UserRegistration registration);
        Task<UserCredential?> LoginAsync(string email, string password);
        Task<bool> EmailExistsAsync(string email);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private static readonly ConcurrentDictionary<string, UserCredential> _users = new();

        public async Task<bool> RegisterUserAsync(UserRegistration registration)
        {
            try
            {
                // Check if email already exists
                if (_users.Values.Any(u => u.Email.Equals(registration.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }

                var passwordHash = HashPassword(registration.Password);

                var credential = new UserCredential
                {
                    Id = registration.Id,
                    Email = registration.Email,
                    PasswordHash = passwordHash,
                    FirstName = registration.FirstName,
                    LastName = registration.LastName,
                    Phone = registration.Phone,
                    DateOfBirth = registration.DateOfBirth,
                    Company = registration.Company,
                    JobTitle = registration.JobTitle
                };

                bool added = _users.TryAdd(credential.Id, credential);
                return await Task.FromResult(added);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return false;
            }
        }

        public async Task<UserCredential?> LoginAsync(string email, string password)
        {
            try
            {
                var user = _users.Values.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    return null;
                }

                // Verify password
                if (VerifyPassword(password, user.PasswordHash))
                {
                    return await Task.FromResult(user);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging in: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var exists = _users.Values.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return await Task.FromResult(exists);
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private static bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hash, StringComparison.Ordinal);
        }
    }
}
