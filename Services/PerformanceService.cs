using System.Runtime.CompilerServices;

namespace EventEaseApp.Services
{
    /// <summary>
    /// Service for handling performance optimizations across the application
    /// </summary>
    public interface IPerformanceService
    {
        void WarmupCaches();
        Task PreloadCriticalDataAsync();
        void LogPerformanceMetric(string operation, TimeSpan duration);
    }

    public class PerformanceService : IPerformanceService
    {
        private readonly IEventService _eventService;
        private readonly Dictionary<string, List<TimeSpan>> _performanceMetrics = new();

        public PerformanceService(IEventService eventService)
        {
            _eventService = eventService;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WarmupCaches()
        {
            // Warmup JIT compilation for commonly used methods
            _ = _eventService.GetAllEventsAsync();
        }

        public async Task PreloadCriticalDataAsync()
        {
            try
            {
                // Preload events to initialize lazy collections
                var startTime = DateTime.UtcNow;
                _ = await _eventService.GetAllEventsAsync();
                LogPerformanceMetric("PreloadEvents", DateTime.UtcNow - startTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Preload error: {ex.Message}");
            }
        }

        public void LogPerformanceMetric(string operation, TimeSpan duration)
        {
            if (!_performanceMetrics.ContainsKey(operation))
            {
                _performanceMetrics[operation] = new List<TimeSpan>();
            }

            _performanceMetrics[operation].Add(duration);

            // Log if duration is concerning (> 100ms)
            if (duration.TotalMilliseconds > 100)
            {
                Console.WriteLine($"Performance warning: {operation} took {duration.TotalMilliseconds:F2}ms");
            }
        }

        public Dictionary<string, double> GetAverageMetrics()
        {
            return _performanceMetrics.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Average(ts => ts.TotalMilliseconds)
            );
        }
    }
}