using AlohaMaui.Core.Entities;

namespace AlohaMaui.Api.Models
{
    public class EventsRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Category { get; set; } = "other";
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Photo { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public Place? Place { get; set; }
        public bool IsOffline { get; set; }
        public string? OnlineDetails { get; set; }
        public bool? FamilyFriendly { get; set; }
        public bool? DogFriendly { get; set; }
        public string? Instagram { get; set; }
        public string? Facebook { get; set; }
        public string ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Website { get; set; }

        public CommunityEvent ToEvent()
        {
            return new CommunityEvent
            {
                Id = Id,
                Title = Title,
                Description = Description,
                Category = Category,
                Price = Price,
                Currency = Currency,
                StartsAt = StartsAt,
                EndsAt = EndsAt,
                Place = Place,
                IsOffline = IsOffline,
                OnlineDetails = OnlineDetails,
                FamilyFriendly = FamilyFriendly,
                DogFriendly = DogFriendly,
                Contacts = new EventContacts
                {
                    Instagram = Instagram,
                    Facebook = Facebook,
                    ContactEmail = ContactEmail,
                    ContactPhone = ContactPhone,
                    Website = Website
                }
            };
        }
    }
}
