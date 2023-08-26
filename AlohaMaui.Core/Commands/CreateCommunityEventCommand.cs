using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Commands
{
    public  class CreateCommunityEventCommand : IRequest<CommunityEvent>
    {
        public Guid UserId { get; }
        public CommunityEvent Event { get; }

        public CreateCommunityEventCommand(Guid userId, CommunityEvent communityEvent)
        {
            UserId = userId;
            Event = communityEvent;
        }
    }
}
