using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Filters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlohaMaui.Core.Queries
{
    public  class GetPublicCommunityEventsQuery : IRequest<IEnumerable<CommunityEvent>>
    {
        public PublicCommunityEventsFilter Filter { get; }

        public GetPublicCommunityEventsQuery(PublicCommunityEventsFilter filter) => Filter = filter;
    }
}
