using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Queries
{
    public class GetPledgeEventTotalsQuery : IRequest<IEnumerable<PledgeTotals>>
    {
        public string GetCacheKey() =>
            $"{nameof(GetPledgeEventTotalsQuery)}";
    }
}
