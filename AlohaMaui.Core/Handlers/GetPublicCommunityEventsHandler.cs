using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Queries;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class GetPublicCommunityEventsHandler : IRequestHandler<GetPublicCommunityEventsQuery, IEnumerable<CommunityEvent>>
    {
        private readonly ICommunityEventRepository _repository;

        public GetPublicCommunityEventsHandler(ICommunityEventRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<CommunityEvent>> Handle(GetPublicCommunityEventsQuery request, CancellationToken cancellationToken)
        {
            return _repository.FindPublicEvents(request.Filter);
        }
    }
}
