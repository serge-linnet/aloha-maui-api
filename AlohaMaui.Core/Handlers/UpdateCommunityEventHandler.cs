using AlohaMaui.Core.Commands;
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class UpdateCommunityEventHandler : IRequestHandler<UpdateCommunityEventCommand, CommunityEvent>
    {
        private readonly ICommunityEventRepository _repository;

        public UpdateCommunityEventHandler(ICommunityEventRepository repository) => _repository = repository;

        public async Task<CommunityEvent> Handle(UpdateCommunityEventCommand request, CancellationToken cancellationToken)
        {
            var newEvent = request.Event;
            
            var result = await _repository.UpdateEvent(newEvent);
            return result;
        }
    }
}
