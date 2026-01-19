using EventEaseApp.Models;
using System.Collections.Concurrent;

namespace EventEaseApp.Services
{
    public interface IEventService
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(int id);
        Task<bool> RegisterForEventAsync(int eventId, string participantName, string email);
        void ClearCache();
    }

    public class EventService : IEventService
    {
        private static readonly ConcurrentDictionary<int, Event> _eventsCache = new();
        private static readonly Lazy<List<Event>> _events = new(() => InitializeEvents());
        private readonly SemaphoreSlim _registrationSemaphore = new(1, 1);

        // Cache for filtered results to avoid recomputation
        private static readonly Dictionary<string, List<Event>> _filterCache = new();
        private static readonly object _filterCacheLock = new();

        private static List<Event> InitializeEvents()
        {
            return new List<Event>
            {
                new Event
                {
                    Id = 1,
                    Name = "Tech Innovation Summit 2026",
                    Date = DateTime.Now.AddDays(15),
                    Location = "San Francisco",
                    Description = "Join industry leaders discussing the latest in AI, blockchain, and quantum computing. Network with innovators and discover breakthrough technologies that will shape the future.",
                    Price = 299.00m,
                    Capacity = 500,
                    RegisteredCount = 234,
                    OrganizerName = "TechEvents Inc.",
                    OrganizerContact = "contact@techevents.com",
                    Tags = new List<string> { "Technology", "AI", "Networking", "Innovation" }
                },
                new Event
                {
                    Id = 2,
                    Name = "Community Food Festival",
                    Date = DateTime.Now.AddDays(7),
                    Location = "New York",
                    Description = "Celebrate diverse cuisines from around the world. Local restaurants, food trucks, and cooking demonstrations all day long. Family-friendly event with live music.",
                    Price = 0,
                    Capacity = 1000,
                    RegisteredCount = 567,
                    OrganizerName = "NYC Community Events",
                    OrganizerContact = "info@nycevents.org",
                    Tags = new List<string> { "Food", "Community", "Family", "Culture" }
                },
                new Event
                {
                    Id = 3,
                    Name = "Digital Marketing Masterclass",
                    Date = DateTime.Now.AddDays(22),
                    Location = "Los Angeles",
                    Description = "Learn advanced strategies for social media, SEO, and content marketing. Hands-on workshops with industry experts and practical exercises.",
                    Price = 149.50m,
                    Capacity = 200,
                    RegisteredCount = 89,
                    OrganizerName = "Marketing Pros Academy",
                    OrganizerContact = "learn@marketingpros.com",
                    Tags = new List<string> { "Marketing", "Digital", "SEO", "Social Media" }
                },
                new Event
                {
                    Id = 4,
                    Name = "Startup Pitch Competition",
                    Date = DateTime.Now.AddDays(30),
                    Location = "Chicago",
                    Description = "Watch promising startups pitch their ideas to investors. Network with entrepreneurs and venture capitalists. Cash prizes for top 3 pitches.",
                    Price = 25.00m,
                    Capacity = 300,
                    RegisteredCount = 156,
                    OrganizerName = "Startup Chicago",
                    OrganizerContact = "events@startupchicago.com",
                    Tags = new List<string> { "Startup", "Investment", "Entrepreneurship", "Competition" }
                },
                new Event
                {
                    Id = 5,
                    Name = "Art & Culture Expo",
                    Date = DateTime.Now.AddDays(12),
                    Location = "New York",
                    Description = "Explore contemporary art installations, meet local artists, and participate in interactive cultural workshops. Live performances throughout the day.",
                    Price = 35.00m,
                    Capacity = 400,
                    RegisteredCount = 278,
                    OrganizerName = "NYC Arts Council",
                    OrganizerContact = "expo@nycartscouncil.org",
                    Tags = new List<string> { "Art", "Culture", "Exhibition", "Workshops" }
                },
                new Event
                {
                    Id = 6,
                    Name = "Blockchain & Crypto Conference",
                    Date = DateTime.Now.AddDays(45),
                    Location = "Miami",
                    Description = "Deep dive into blockchain technology, cryptocurrency trends, and DeFi innovations. Featuring keynotes from industry pioneers and hands-on workshops.",
                    Price = 399.00m,
                    Capacity = 800,
                    RegisteredCount = 456,
                    OrganizerName = "CryptoWorld Events",
                    OrganizerContact = "info@cryptoworldevents.com",
                    Tags = new List<string> { "Blockchain", "Cryptocurrency", "DeFi", "Technology" }
                },
                new Event
                {
                    Id = 7,
                    Name = "Fitness & Wellness Bootcamp",
                    Date = DateTime.Now.AddDays(5),
                    Location = "Austin",
                    Description = "Transform your health with expert-led fitness sessions, nutrition workshops, and mental wellness seminars. All fitness levels welcome.",
                    Price = 75.00m,
                    Capacity = 150,
                    RegisteredCount = 89,
                    OrganizerName = "Austin Wellness Center",
                    OrganizerContact = "events@austinwellness.com",
                    Tags = new List<string> { "Fitness", "Wellness", "Health", "Bootcamp" }
                },
                new Event
                {
                    Id = 8,
                    Name = "Jazz Under the Stars",
                    Date = DateTime.Now.AddDays(18),
                    Location = "New Orleans",
                    Description = "An enchanting evening of live jazz music in an outdoor setting. Local and touring musicians performing classic and contemporary pieces.",
                    Price = 45.00m,
                    Capacity = 250,
                    RegisteredCount = 187,
                    OrganizerName = "New Orleans Music Society",
                    OrganizerContact = "tickets@nolamusic.org",
                    Tags = new List<string> { "Music", "Jazz", "Outdoor", "Concert" }
                },
                new Event
                {
                    Id = 9,
                    Name = "Sustainable Living Workshop",
                    Date = DateTime.Now.AddDays(25),
                    Location = "Portland",
                    Description = "Learn practical tips for eco-friendly living, sustainable fashion, zero-waste practices, and renewable energy solutions for your home.",
                    Price = 0,
                    Capacity = 100,
                    RegisteredCount = 67,
                    OrganizerName = "Green Portland Initiative",
                    OrganizerContact = "workshops@greenportland.org",
                    Tags = new List<string> { "Sustainability", "Environment", "Workshop", "Green Living" }
                },
                new Event
                {
                    Id = 10,
                    Name = "Photography Masterclass: Urban Landscapes",
                    Date = DateTime.Now.AddDays(35),
                    Location = "Seattle",
                    Description = "Capture stunning urban photography with professional techniques. Morning theory session followed by guided photo walk through the city.",
                    Price = 120.00m,
                    Capacity = 30,
                    RegisteredCount = 23,
                    OrganizerName = "Seattle Photo Academy",
                    OrganizerContact = "classes@seattlephoto.com",
                    Tags = new List<string> { "Photography", "Urban", "Masterclass", "Art" }
                },
                new Event
                {
                    Id = 11,
                    Name = "Wine Tasting & Vineyard Tour",
                    Date = DateTime.Now.AddDays(40),
                    Location = "Napa Valley",
                    Description = "Discover exceptional wines from local vineyards. Guided tastings, winemaking insights, and gourmet food pairings in beautiful vineyard settings.",
                    Price = 185.00m,
                    Capacity = 60,
                    RegisteredCount = 42,
                    OrganizerName = "Napa Valley Tours",
                    OrganizerContact = "bookings@napavalleytours.com",
                    Tags = new List<string> { "Wine", "Tasting", "Food", "Tourism" }
                },
                new Event
                {
                    Id = 12,
                    Name = "Gaming & Esports Tournament",
                    Date = DateTime.Now.AddDays(28),
                    Location = "Las Vegas",
                    Description = "Competitive gaming tournament featuring multiple popular titles. Prize pools, streaming, and meet & greets with professional gamers.",
                    Price = 50.00m,
                    Capacity = 500,
                    RegisteredCount = 312,
                    OrganizerName = "Vegas Gaming Arena",
                    OrganizerContact = "tournaments@vegasgaming.com",
                    Tags = new List<string> { "Gaming", "Esports", "Competition", "Technology" }
                },
                new Event
                {
                    Id = 13,
                    Name = "Mindfulness & Meditation Retreat",
                    Date = DateTime.Now.AddDays(50),
                    Location = "Sedona",
                    Description = "Weekend retreat focused on mindfulness practices, guided meditation, and personal wellness. Set in the serene red rock landscape of Sedona.",
                    Price = 350.00m,
                    Capacity = 40,
                    RegisteredCount = 28,
                    OrganizerName = "Sedona Wellness Retreats",
                    OrganizerContact = "info@sedonawellness.com",
                    Tags = new List<string> { "Mindfulness", "Meditation", "Retreat", "Wellness" }
                },
                new Event
                {
                    Id = 14,
                    Name = "Science Fiction Convention",
                    Date = DateTime.Now.AddDays(60),
                    Location = "Denver",
                    Description = "Celebrate sci-fi culture with author panels, cosplay contests, technology demos, and screenings of classic and new science fiction films.",
                    Price = 65.00m,
                    Capacity = 1200,
                    RegisteredCount = 789,
                    OrganizerName = "Mile High Sci-Fi",
                    OrganizerContact = "convention@milehighscifi.com",
                    Tags = new List<string> { "Science Fiction", "Convention", "Cosplay", "Entertainment" }
                },
                new Event
                {
                    Id = 15,
                    Name = "Cooking Class: Italian Cuisine",
                    Date = DateTime.Now.AddDays(20),
                    Location = "Boston",
                    Description = "Learn authentic Italian cooking techniques from a professional chef. Hands-on preparation of pasta, sauces, and traditional desserts.",
                    Price = 95.00m,
                    Capacity = 24,
                    RegisteredCount = 18,
                    OrganizerName = "Boston Culinary Institute",
                    OrganizerContact = "classes@bostonculi.edu",
                    Tags = new List<string> { "Cooking", "Italian", "Cuisine", "Class" }
                }
            };
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            // Return cached data immediately for better performance
            return await Task.FromResult(_events.Value);
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            // Use cache for better performance
            if (_eventsCache.TryGetValue(id, out var cachedEvent))
            {
                return cachedEvent;
            }

            var eventItem = _events.Value.FirstOrDefault(e => e.Id == id);
            if (eventItem != null)
            {
                _eventsCache.TryAdd(id, eventItem);
            }

            return await Task.FromResult(eventItem);
        }

        public async Task<bool> RegisterForEventAsync(int eventId, string participantName, string email)
        {
            await _registrationSemaphore.WaitAsync();
            try
            {
                var eventItem = _events.Value.FirstOrDefault(e => e.Id == eventId);
                if (eventItem != null && !eventItem.IsFull)
                {
                    eventItem.RegisteredCount++;

                    // Update cache if exists
                    if (_eventsCache.TryGetValue(eventId, out var cachedEvent))
                    {
                        cachedEvent.RegisteredCount = eventItem.RegisteredCount;
                    }

                    return true;
                }
                return false;
            }
            finally
            {
                _registrationSemaphore.Release();
            }
        }

        public void ClearCache()
        {
            _eventsCache.Clear();
            lock (_filterCacheLock)
            {
                _filterCache.Clear();
            }
        }
    }
}