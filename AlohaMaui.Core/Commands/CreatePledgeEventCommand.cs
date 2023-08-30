using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Commands
{
    public class CreatePledgeEventCommand : IRequest<PledgeEvent>
    {
        public PledgeEvent Pledge { get; }

        public CreatePledgeEventCommand(PledgeEvent pledge) => Pledge = pledge;
    }
}
