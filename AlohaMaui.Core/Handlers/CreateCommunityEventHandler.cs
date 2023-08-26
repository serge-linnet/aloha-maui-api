using AlohaMaui.Core.Commands;
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class CreateCommunityEventHandler : IRequestHandler<CreateCommunityEventCommand, CommunityEvent>
    {
        private readonly ICommunityEventRepository _repository;

        public CreateCommunityEventHandler(ICommunityEventRepository repository) => _repository = repository;

        public async Task<CommunityEvent> Handle(CreateCommunityEventCommand request, CancellationToken cancellationToken)
        {
            var newEvent = request.Event;
            newEvent.Id = Guid.NewGuid();
            newEvent.UserId = request.UserId;

            var result = await _repository.CreateEvent(newEvent);
            return result;
        }
    }
}
