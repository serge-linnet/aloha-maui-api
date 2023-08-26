using AlohaMaui.Core.Commands;
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class ChangeCommunityEventStatusHandler : IRequestHandler<ChangeCommunityEventStatusCommand, CommunityEvent>
    {
        private readonly ICommunityEventRepository _repository;

        public ChangeCommunityEventStatusHandler(ICommunityEventRepository repository) => _repository = repository;

        public async Task<CommunityEvent> Handle(ChangeCommunityEventStatusCommand request, CancellationToken cancellationToken)
        {
            var evnt = await _repository.Find(request.EventId);
            evnt.Status = request.Status;
            return await _repository.Update(evnt);
        }
    }
}
