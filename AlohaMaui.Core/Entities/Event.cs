using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class Event
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string? Category { get; set; }

        [JsonProperty(PropertyName = "price")]
        public decimal? Price { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string? Currency { get; set; }

        [JsonProperty(PropertyName = "startsAt")]
        public DateTime? StartsAt { get; set; }

        [JsonProperty(PropertyName = "endsAt")]
        public DateTime? EndsAt { get; set; }

        [JsonProperty(PropertyName = "place")]
        public Place Place { get; set; }

        [JsonProperty(PropertyName = "status")]
        public EventStatus Status { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "onlineDetails")]
        public string OnlineDetails { get; set; }

        [JsonProperty(PropertyName = "familyFriendly")]
        public bool? FamilyFriendly { get; set; }

        [JsonProperty(PropertyName = "dogFriendly")]
        public bool? DogFriendly { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        [JsonProperty(PropertyName = "assets")]
        public EventAssets? Assets { get; set; }

        [JsonProperty(PropertyName = "contacts")]
        public EventContacts? Contacts { get; set; }
    }
}
