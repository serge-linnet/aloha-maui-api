namespace AlohaMaui.Core.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; } = "USD";
        public string PhotoUrl { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public Place Place { get; set; }
        public EventStatus Status { get; set; }
    }

    public enum EventStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
