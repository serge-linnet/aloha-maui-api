using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Queries;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class GetAllPledgeEventsHandler : IRequestHandler<GetAllPledgeEventsQuery, IEnumerable<PledgeEvent>>
    {
        private readonly IPledgeEventRepository _repository;

        public GetAllPledgeEventsHandler(IPledgeEventRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<PledgeEvent>> Handle(GetAllPledgeEventsQuery request, CancellationToken cancellationToken)
        {
            return _repository.FindAll();
        }
    }
}
