namespace AlohaMaui.Core.Filters
{
    public record PublicCommunityEventsFilter
    {
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
        public bool? FamilyFriendly { get; set; }
        public bool? DogFriendly { get; set; }
        public string? Country { get; set; }
        public bool? IsOffline { get; set; }
    }
}
