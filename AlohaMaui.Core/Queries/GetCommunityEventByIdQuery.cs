using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Queries
{
    public class GetCommunityEventByIdQuery : IRequest<CommunityEvent>
    {
        public Guid EventId { get; }

        public GetCommunityEventByIdQuery(Guid eventId) => EventId = eventId;
    }
}
