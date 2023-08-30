using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Queries
{
    public class GetAllPledgeEventsQuery : IRequest<IEnumerable<PledgeEvent>>
    {
        public string GetCacheKey() =>
            $"{nameof(GetAllPledgeEventsQuery)}";
    }

    public class GetPledgeEventTotalsQuery : IRequest<IEnumerable<PledgeTotals>>
    {
        public string GetCacheKey() =>
            $"{nameof(GetPledgeEventTotalsQuery)}";
    }
}
