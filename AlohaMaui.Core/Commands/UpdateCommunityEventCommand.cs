using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Commands
{
    public class UpdateCommunityEventCommand : IRequest<CommunityEvent>
    {
        public Guid UserId { get; }
        public CommunityEvent Event { get; }

        public UpdateCommunityEventCommand(CommunityEvent communityEvent)
        {
            Event = communityEvent;
        }
    }
}
