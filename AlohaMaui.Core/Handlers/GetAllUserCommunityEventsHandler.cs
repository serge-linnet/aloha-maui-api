using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Queries;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class GetAllUserCommunityEventsHandler : IRequestHandler<GetAllUserCommunityEventsQuery, IEnumerable<CommunityEvent>>
    {
        private readonly ICommunityEventRepository _repository;

        public GetAllUserCommunityEventsHandler(ICommunityEventRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<CommunityEvent>> Handle(GetAllUserCommunityEventsQuery request, CancellationToken cancellationToken)
        {
            return _repository.FindEventsForUser(request.UserId);
        }
    }
}
