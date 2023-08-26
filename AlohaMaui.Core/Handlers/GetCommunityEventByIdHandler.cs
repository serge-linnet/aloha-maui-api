using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Queries;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class GetCommunityEventByIdHandler : IRequestHandler<GetCommunityEventByIdQuery, CommunityEvent>
    {
        private readonly ICommunityEventRepository _repository;

        public GetCommunityEventByIdHandler(ICommunityEventRepository repository)
        {
            _repository = repository;
        }

        public async Task<CommunityEvent> Handle(GetCommunityEventByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.Find(request.EventId);
        }
    }
}
