using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Queries
{
    public class GetAllOrgEventsQuery : IRequest<IEnumerable<OrganizationEvent>>
    {
        public string GetCacheKey() =>
            $"{nameof(GetAllOrgEventsQuery)}";
    }
}
