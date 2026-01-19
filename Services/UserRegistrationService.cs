using EventEaseApp.Models;
using System.Collections.Concurrent;

namespace EventEaseApp.Services
{
    public interface IUserRegistrationService
    {
        Task<bool> RegisterUserAsync(UserRegistration registration);
        Task<UserRegistration?> GetUserByEmailAsync(string email);
        Task<UserRegistration?> GetUserByIdAsync(string userId);
        Task<List<UserRegistration>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(UserRegistration registration);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> EmailExistsAsync(string email);
    }

    public class UserRegistrationService : IUserRegistrationService
    {
        private static readonly ConcurrentDictionary<string, UserRegistration> _users = new();

        public async Task<bool> RegisterUserAsync(UserRegistration registration)
        {
            try
            {
                // Check if email already exists
                if (await EmailExistsAsync(registration.Email))
                {
                    return false;
                }

                registration.Id = Guid.NewGuid().ToString();
                registration.RegistrationDate = DateTime.Now;

                return _users.TryAdd(registration.Id, registration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return false;
            }
        }

        public async Task<UserRegistration?> GetUserByEmailAsync(string email)
        {
            await Task.CompletedTask;
            return _users.Values.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<UserRegistration?> GetUserByIdAsync(string userId)
        {
            await Task.CompletedTask;
            return _users.TryGetValue(userId, out var user) ? user : null;
        }

        public async Task<List<UserRegistration>> GetAllUsersAsync()
        {
            await Task.CompletedTask;
            return _users.Values.ToList();
        }

        public async Task<bool> UpdateUserAsync(UserRegistration registration)
        {
            await Task.CompletedTask;
            if (_users.ContainsKey(registration.Id))
            {
                _users[registration.Id] = registration;
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            await Task.CompletedTask;
            return _users.TryRemove(userId, out _);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            await Task.CompletedTask;
            return _users.Values.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
    }
}