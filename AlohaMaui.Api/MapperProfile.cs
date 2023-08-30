using AlohaMaui.Api.Models;
using AlohaMaui.Core.Entities;
using AutoMapper;

namespace AlohaMaui.Api
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UpdateCommunityEventRequest, CommunityEvent>();
            CreateMap<PledgeEventModel, PledgeEvent>();
            CreateMap<OrganizationEventModel, OrganizationEvent>();
        }
    }
}
