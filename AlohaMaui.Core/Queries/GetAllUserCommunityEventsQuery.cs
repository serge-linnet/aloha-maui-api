using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Queries
{
    public class GetAllUserCommunityEventsQuery : IRequest<IEnumerable<CommunityEvent>>
    {
        public Guid UserId { get; }

        public GetAllUserCommunityEventsQuery(Guid userId) => UserId = userId;

        public string GetCacheKey() =>
            $"{nameof(GetAllUserCommunityEventsQuery)}-{UserId}";
    }
}
