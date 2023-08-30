using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Queries;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class GetOrgEventsHandler : IRequestHandler<GetAllOrgEventsQuery, IEnumerable<OrganizationEvent>>
    {
        private readonly IOrgEventRepository _repository;

        public GetOrgEventsHandler(IOrgEventRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<OrganizationEvent>> Handle(GetAllOrgEventsQuery request, CancellationToken cancellationToken)
        {
            return _repository.FindAll();
        }
    }
}
