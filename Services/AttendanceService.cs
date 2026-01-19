using EventEaseApp.Models;
using System.Collections.Concurrent;

namespace EventEaseApp.Services
{
    public delegate void RegistrationChangedEventHandler(string userId, int eventId);

    public interface IAttendanceService
    {
        Task<bool> RegisterForEventAsync(string userId, int eventId);
        Task<bool> CheckInUserAsync(string userId, int eventId);
        Task<bool> MarkNoShowAsync(string userId, int eventId);
        Task<EventRegistration?> GetRegistrationAsync(string userId, int eventId);
        Task<List<EventRegistration>> GetUserRegistrationsAsync(string userId);
        Task<List<EventRegistration>> GetEventRegistrationsAsync(int eventId);
        Task<int> GetEventAttendanceCountAsync(int eventId);
        Task<List<EventRegistration>> GetAllRegistrationsAsync();
        Task<Dictionary<string, int>> GetAttendanceStatsAsync();
        event RegistrationChangedEventHandler? OnRegistrationChanged;
    }

    public class AttendanceService : IAttendanceService
    {
        private static readonly ConcurrentDictionary<string, EventRegistration> _registrations = new();
        private readonly IEventService _eventService;
        public event RegistrationChangedEventHandler? OnRegistrationChanged;

        public AttendanceService(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<bool> RegisterForEventAsync(string userId, int eventId)
        {
            try
            {
                // Check if user is already registered for this event
                var existingRegistration = _registrations.Values
                    .FirstOrDefault(r => r.UserId == userId && r.EventId == eventId);

                if (existingRegistration != null)
                {
                    return false; // Already registered
                }

                // Check if event exists and has capacity
                var eventItem = await _eventService.GetEventByIdAsync(eventId);
                if (eventItem == null || eventItem.IsFull)
                {
                    return false;
                }

                var registration = new EventRegistration
                {
                    UserId = userId,
                    EventId = eventId,
                    RegistrationDate = DateTime.Now,
                    Status = "Registered"
                };

                bool added = _registrations.TryAdd(registration.Id, registration);

                if (added)
                {
                    // Update event registered count
                    await _eventService.RegisterForEventAsync(eventId, "", "");
                    // Notify listeners
                    OnRegistrationChanged?.Invoke(userId, eventId);
                }

                return added;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering for event: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CheckInUserAsync(string userId, int eventId)
        {
            await Task.CompletedTask;
            var registration = _registrations.Values
                .FirstOrDefault(r => r.UserId == userId && r.EventId == eventId);

            if (registration != null && registration.Status == "Registered")
            {
                registration.Status = "CheckedIn";
                return true;
            }
            return false;
        }

        public async Task<bool> MarkNoShowAsync(string userId, int eventId)
        {
            await Task.CompletedTask;
            var registration = _registrations.Values
                .FirstOrDefault(r => r.UserId == userId && r.EventId == eventId);

            if (registration != null && registration.Status == "Registered")
            {
                registration.Status = "NoShow";
                return true;
            }
            return false;
        }

        public async Task<EventRegistration?> GetRegistrationAsync(string userId, int eventId)
        {
            await Task.CompletedTask;
            return _registrations.Values
                .FirstOrDefault(r => r.UserId == userId && r.EventId == eventId);
        }

        public async Task<List<EventRegistration>> GetUserRegistrationsAsync(string userId)
        {
            await Task.CompletedTask;
            return _registrations.Values
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RegistrationDate)
                .ToList();
        }

        public async Task<List<EventRegistration>> GetEventRegistrationsAsync(int eventId)
        {
            await Task.CompletedTask;
            return _registrations.Values
                .Where(r => r.EventId == eventId)
                .OrderBy(r => r.RegistrationDate)
                .ToList();
        }

        public async Task<int> GetEventAttendanceCountAsync(int eventId)
        {
            await Task.CompletedTask;
            return _registrations.Values
                .Count(r => r.EventId == eventId && r.Status == "CheckedIn");
        }

        public async Task<List<EventRegistration>> GetAllRegistrationsAsync()
        {
            await Task.CompletedTask;
            return _registrations.Values.ToList();
        }

        public async Task<Dictionary<string, int>> GetAttendanceStatsAsync()
        {
            await Task.CompletedTask;
            var stats = new Dictionary<string, int>
            {
                ["TotalRegistrations"] = _registrations.Count,
                ["CheckedIn"] = _registrations.Values.Count(r => r.Status == "CheckedIn"),
                ["NoShows"] = _registrations.Values.Count(r => r.Status == "NoShow"),
                ["PendingCheckIn"] = _registrations.Values.Count(r => r.Status == "Registered")
            };
            return stats;
        }
    }
}