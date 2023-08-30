using AlohaMaui.Core.Commands;
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class CreatePledgeEventHandler : IRequestHandler<CreatePledgeEventCommand, PledgeEvent>
    {
        private readonly IPledgeEventRepository _repository;

        public CreatePledgeEventHandler(IPledgeEventRepository repository) => _repository = repository;

        public async Task<PledgeEvent> Handle(CreatePledgeEventCommand request, CancellationToken cancellationToken)
        {
            var newEvent = request.Pledge;
            newEvent.Id = Guid.NewGuid();
            newEvent.Type = EventType.Pledge;
            newEvent.Status = EventStatus.Approved;

            var result = await _repository.Create(newEvent);
            return result;
        }
    }
}
