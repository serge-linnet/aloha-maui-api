using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Queries;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class GetPledgeEventTotalsHandler : IRequestHandler<GetPledgeEventTotalsQuery, IEnumerable<PledgeTotals>>
    {
        private readonly IPledgeEventRepository _repository;

        public GetPledgeEventTotalsHandler(IPledgeEventRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<PledgeTotals>> Handle(GetPledgeEventTotalsQuery request, CancellationToken cancellationToken)
        {
            return _repository.FindTotals();
        }
    }
}
