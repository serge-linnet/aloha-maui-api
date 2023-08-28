using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Filters;
using MediatR;

namespace AlohaMaui.Core.Queries
{
    public class GetCommunityEventsForAdminQuery : IRequest<IEnumerable<CommunityEvent>>
    {

        public GetCommunityEventsForAdminQuery(ManageCommunityEventsFilter filter) => Filter = filter;

        public ManageCommunityEventsFilter Filter { get; }
    }
}
