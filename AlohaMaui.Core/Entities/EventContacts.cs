using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class EventContacts
    {
        [JsonProperty(PropertyName = "instagram")]
        public string? Instagram { get; set; }

        [JsonProperty(PropertyName = "facebook")]
        public string? Facebook { get; set; }

        [JsonProperty(PropertyName = "contactEmail")]
        public string? ContactEmail { get; set; }

        [JsonProperty(PropertyName = "contactPhone")]
        public string? ContactPhone { get; set; }

        [JsonProperty(PropertyName = "website")]
        public string? Website { get; set; }
    }
}
