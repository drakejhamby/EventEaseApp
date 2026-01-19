using EventEaseApp.Models;
using System.Collections.Concurrent;

namespace EventEaseApp.Services
{
    public interface IUserSessionService
    {
        Task<UserSession?> CreateSessionAsync(string email, string fullName);
        Task<UserSession?> GetSessionAsync(string sessionId);
        Task<UserSession?> GetCurrentSessionAsync();
        Task<bool> UpdateLastActivityAsync(string sessionId);
        Task<bool> EndSessionAsync(string sessionId);
        Task<bool> IsSessionActiveAsync(string sessionId);
        Task<List<UserSession>> GetActiveSessionsAsync();
        event Action<UserSession> OnSessionCreated;
        event Action<string> OnSessionEnded;
    }

    public class UserSessionService : IUserSessionService
    {
        private static readonly ConcurrentDictionary<string, UserSession> _sessions = new();
        private static UserSession? _currentSession;
        private readonly Timer _cleanupTimer;

        public event Action<UserSession>? OnSessionCreated;
        public event Action<string>? OnSessionEnded;

        public UserSessionService()
        {
            // Cleanup expired sessions every 30 minutes
            _cleanupTimer = new Timer(CleanupExpiredSessions, null, TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30));
        }

        public async Task<UserSession?> CreateSessionAsync(string email, string fullName)
        {
            try
            {
                // End any existing session for this user
                var existingSession = _sessions.Values.FirstOrDefault(s => s.Email == email && s.IsActive);
                if (existingSession != null)
                {
                    await EndSessionAsync(existingSession.SessionId);
                }

                var session = new UserSession
                {
                    UserId = email, // Using email as user ID for simplicity
                    Email = email,
                    FullName = fullName,
                    LoginTime = DateTime.Now,
                    LastActivity = DateTime.Now,
                    IsActive = true
                };

                _sessions.TryAdd(session.SessionId, session);
                _currentSession = session;

                OnSessionCreated?.Invoke(session);
                return session;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating session: {ex.Message}");
                return null;
            }
        }

        public async Task<UserSession?> GetSessionAsync(string sessionId)
        {
            await Task.CompletedTask;
            return _sessions.TryGetValue(sessionId, out var session) && session.IsActive ? session : null;
        }

        public async Task<UserSession?> GetCurrentSessionAsync()
        {
            await Task.CompletedTask;
            return _currentSession?.IsActive == true ? _currentSession : null;
        }

        public async Task<bool> UpdateLastActivityAsync(string sessionId)
        {
            await Task.CompletedTask;
            if (_sessions.TryGetValue(sessionId, out var session) && session.IsActive)
            {
                session.LastActivity = DateTime.Now;
                return true;
            }
            return false;
        }

        public async Task<bool> EndSessionAsync(string sessionId)
        {
            await Task.CompletedTask;
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                session.IsActive = false;
                if (_currentSession?.SessionId == sessionId)
                {
                    _currentSession = null;
                }
                OnSessionEnded?.Invoke(sessionId);
                return true;
            }
            return false;
        }

        public async Task<bool> IsSessionActiveAsync(string sessionId)
        {
            await Task.CompletedTask;
            return _sessions.TryGetValue(sessionId, out var session) && session.IsActive;
        }

        public async Task<List<UserSession>> GetActiveSessionsAsync()
        {
            await Task.CompletedTask;
            return _sessions.Values.Where(s => s.IsActive).ToList();
        }

        private void CleanupExpiredSessions(object? state)
        {
            try
            {
                var expiredTime = DateTime.Now.AddHours(-24); // Sessions expire after 24 hours
                var expiredSessions = _sessions.Values
                    .Where(s => s.LastActivity < expiredTime)
                    .ToList();

                foreach (var session in expiredSessions)
                {
                    _ = EndSessionAsync(session.SessionId);
                }
            }
            catch (Exception)
            {
                // ignore cleanup errors
            }
        }
    }
}