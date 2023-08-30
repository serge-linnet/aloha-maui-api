using AlohaMaui.Core.Commands;
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class CreateOrgEventHandler : IRequestHandler<CreateOrgEventCommand, OrganizationEvent>
    {
        private readonly IOrgEventRepository _repository;

        public CreateOrgEventHandler(IOrgEventRepository repository) => _repository = repository;

        public async Task<OrganizationEvent> Handle(CreateOrgEventCommand request, CancellationToken cancellationToken)
        {
            var newEvent = request.Event;
            newEvent.Id = Guid.NewGuid();
            newEvent.Type = EventType.OrgEvent;
            newEvent.Status = EventStatus.Approved;

            var result = await _repository.Create(newEvent);
            return result;
        }
    }
}
