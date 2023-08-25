using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class EventAssets
    {
        [JsonProperty]
        public string? CoverPhoto { get; set; }

        [JsonProperty]
        public string? Thumbnail { get; set; }
    }
}
