using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Queries;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class GetCommunityEventsForAdminHandler : IRequestHandler<GetCommunityEventsForAdminQuery, IEnumerable<CommunityEvent>>
    {
        private readonly ICommunityEventRepository _repository;

        public GetCommunityEventsForAdminHandler(ICommunityEventRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<CommunityEvent>> Handle(GetCommunityEventsForAdminQuery request, CancellationToken cancellationToken)
        {
            return _repository.FindEventsForAdmin(request.Filter);
        }
    }
}
