using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Commands
{
    public class CreateCommunityEventAssetsCommand : IRequest<EventAssets>
    {
        public Guid EventId { get; }
        public string ImageBase64 { get; }

        public CreateCommunityEventAssetsCommand(Guid eventId, string imageBase64)
        {
            EventId = eventId;
            ImageBase64 = imageBase64;
        }
    }
}
