using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class OrganizationEvent : EventBase
    {
        [JsonProperty]
        public string? Country { get; set; }

        [JsonProperty]
        public string? City { get; set; }

        [JsonProperty]
        public string? GroupName { get; set; }

        [JsonProperty]
        public string? OrgType { get; set; }

        [JsonProperty]
        public int? NumberOfPeople { get; set; }

        public OrganizationEvent()
        {
            Type = EventType.OrgEvent;
            Status = EventStatus.Approved;
        }
    }
}
