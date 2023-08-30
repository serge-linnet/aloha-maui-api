using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class EventBase
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonProperty(PropertyName = "type")]
        public EventType Type { get; set; }

        [JsonProperty]
        public EventStatus Status { get; set; }
    }
}
