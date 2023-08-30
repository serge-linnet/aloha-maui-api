using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Commands
{
    public class ChangeCommunityEventStatusCommand : IRequest<CommunityEvent>
    {
        public Guid EventId { get; }
        public EventStatus Status { get; }

        public ChangeCommunityEventStatusCommand(Guid eventId, EventStatus status)
        {
            EventId = eventId;
            Status = status;
        }
    }
}
