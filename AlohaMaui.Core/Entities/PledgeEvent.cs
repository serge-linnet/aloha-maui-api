using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class PledgeTotals
    {
        public string Country { get; set; }
        public int People { get; set; }
        public int Pledges { get; set; }
    }
    public class PledgeEvent : EventBase
    {
        [JsonProperty]
        public string? Country { get; set; }

        [JsonProperty]
        public string? City { get; set; }

        [JsonProperty]
        public string? Name { get; set; }

        [JsonProperty]
        public string? Activity { get; set; }

        [JsonProperty]
        public int? NumberOfPeople { get; set; }

        public PledgeEvent()
        {
            Type = EventType.Pledge;
            Status = EventStatus.Approved;
        }
    }
}
