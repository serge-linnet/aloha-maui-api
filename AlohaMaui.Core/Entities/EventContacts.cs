using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class EventContacts
    {
        [JsonProperty]
        public string? Instagram { get; set; }

        [JsonProperty(PropertyName = "facebook")]
        public string? Facebook { get; set; }

        [JsonProperty]
        public string? ContactEmail { get; set; }

        [JsonProperty]
        public string? ContactPhone { get; set; }

        [JsonProperty]
        public string? Website { get; set; }
    }
}
