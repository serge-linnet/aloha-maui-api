using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Commands
{
    public class ChangeCommunityEventStatusCommand : IRequest<CommunityEvent>
    {
        public Guid EventId { get; }
        public CommunityEventStatus Status { get; }

        public ChangeCommunityEventStatusCommand(Guid eventId, CommunityEventStatus status)
        {
            EventId = eventId;
            Status = status;
        }
    }
}
