using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class Place
    {
        [JsonProperty(PropertyName = "address")]
        public string? Address { get; set; }

        [JsonProperty(PropertyName = "postcode")]
        public string? Postcode { get; set; }

        [JsonProperty(PropertyName = "locality")]
        public string? Locality { get; set; }

        [JsonProperty(PropertyName = "region")]
        public string? Region { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string? Country { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public double? Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public double? Longitude { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public PlaceMetadata? Metadata { get; set; }
    }

    public class PlaceMetadata
    {
        [JsonProperty(PropertyName = "googlePlaceId")]
        public string? GooglePlaceId { get; set; }
    }
}
