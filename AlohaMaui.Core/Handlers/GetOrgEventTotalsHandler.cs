using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Queries;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class GetOrgEventTotalsHandler : IRequestHandler<GetOrgEventTotalsQuery, IEnumerable<OrgEventTotals>>
    {
        private readonly IOrgEventRepository _repository;

        public GetOrgEventTotalsHandler(IOrgEventRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<OrgEventTotals>> Handle(GetOrgEventTotalsQuery request, CancellationToken cancellationToken)
        {
            return _repository.FindTotals();
        }
    }
}
