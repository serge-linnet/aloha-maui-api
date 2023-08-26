namespace AlohaMaui.Core.Filters
{
    public class PublicCommunityEventsFilter
    {
        public DateTime From { get; set; } = DateTime.UtcNow.Date;
        public DateTime? To { get; set; }
        public bool? FamilyFriendly { get; set; }
        public bool? DogFriendly { get; set; }
        public string? Country { get; set; }
    }
}
