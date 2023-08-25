using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{

    public class CommunityEvent : EventBase
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonProperty]
        public string Title { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public string? Category { get; set; }

        [JsonProperty]
        public decimal? Price { get; set; }

        [JsonProperty]
        public string? Currency { get; set; }

        [JsonProperty]
        public DateTime? StartsAt { get; set; }

        [JsonProperty]
        public DateTime? EndsAt { get; set; }

        [JsonProperty]
        public Place? Place { get; set; }

        [JsonProperty]
        public CommunityEventStatus Status { get; set; }

        [JsonProperty]
        public bool? IsOffline { get; set; }

        [JsonProperty]
        public string? OnlineDetails { get; set; }

        [JsonProperty]
        public bool? FamilyFriendly { get; set; }

        [JsonProperty]
        public bool? DogFriendly { get; set; }

        [JsonProperty]
        public Guid UserId { get; set; }

        [JsonProperty]
        public EventAssets? Assets { get; set; }

        [JsonProperty]
        public EventContacts? Contacts { get; set; }

        public CommunityEvent()
        {
            Type = EventType.Community;
        }
    }
}
