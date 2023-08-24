using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class EventAssets
    {
        [JsonProperty(PropertyName = "coverPhoto")]
        public string? CoverPhoto { get; set; }

        [JsonProperty(PropertyName = "thumbnail")]
        public string? Thumbnail { get; set; }
    }
}
