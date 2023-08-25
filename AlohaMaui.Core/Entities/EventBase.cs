using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class EventBase
    {
        [JsonProperty(PropertyName = "type")]
        public EventType Type { get; set; }
    }
}
