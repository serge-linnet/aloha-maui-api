using AlohaMaui.Core.Entities;

namespace AlohaMaui.Api.Models
{
    public class UpdateCommunityEventRequest : CommunityEvent
    {
        public string Photo { get; set; }
    }
}
