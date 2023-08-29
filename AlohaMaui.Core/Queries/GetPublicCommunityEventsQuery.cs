using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Filters;
using MediatR;
using Newtonsoft.Json;

namespace AlohaMaui.Core.Queries
{
    public  class GetPublicCommunityEventsQuery : IRequest<IEnumerable<CommunityEvent>>
    {
        public PublicCommunityEventsFilter Filter { get; }

        public GetPublicCommunityEventsQuery(PublicCommunityEventsFilter filter) => Filter = filter;

        public string GetCacheKey()
        {
            return $"{GetType().Name}_{JsonConvert.SerializeObject(Filter)}";
        }
    }
}
