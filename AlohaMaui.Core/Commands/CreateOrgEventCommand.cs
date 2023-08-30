using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Commands
{
    public class CreateOrgEventCommand : IRequest<OrganizationEvent>
    {
        public OrganizationEvent Event { get; }

        public CreateOrgEventCommand(OrganizationEvent oe) => Event = oe;
    }
}
