namespace AlohaMaui.Core.Entities
{
    public class Place
    {
        public string? Address { get; set; }
        public string? Postcode { get; set; }
        public string? Locality { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public PlaceMetadata? Metadata { get; set; }
    }

    public class PlaceMetadata
    {
        public string? GooglePlaceId { get; set; }
    }
}
