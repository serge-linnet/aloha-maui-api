using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Queries
{
    public class GetOrgEventTotalsQuery : IRequest<IEnumerable<OrgEventTotals>>
    {
        public string GetCacheKey() =>
            $"{nameof(GetOrgEventTotalsQuery)}";
    }
}
