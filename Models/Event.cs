using System;
using System.Collections.Generic;

namespace EventEaseApp.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public int RegisteredCount { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public string OrganizerName { get; set; } = string.Empty;
        public string OrganizerContact { get; set; } = string.Empty;

        public bool IsFull => RegisteredCount >= Capacity;
        public int AvailableSpots => Math.Max(0, Capacity - RegisteredCount);
        public bool IsUpcoming => Date > DateTime.UtcNow;
    }
}