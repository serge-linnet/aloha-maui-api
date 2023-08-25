using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class Place
    {
        [JsonProperty]
        public string? PlaceName { get; set; }

        [JsonProperty]
        public string? Address { get; set; }

        [JsonProperty]
        public string? Postcode { get; set; }

        [JsonProperty]
        public string? Locality { get; set; }

        [JsonProperty]
        public string? Region { get; set; }

        [JsonProperty]
        public string? Country { get; set; }

        [JsonProperty]
        public string? CountryCode { get; set; }

        [JsonProperty]
        public double? Latitude { get; set; }

        [JsonProperty]
        public double? Longitude { get; set; }

        [JsonProperty]
        public PlaceMetadata? Metadata { get; set; }
    }

    public class PlaceMetadata
    {
        [JsonProperty(PropertyName = "googlePlaceId")]
        public string? GooglePlaceId { get; set; }
    }
}
