using AlohaMaui.Core.Entities;
using MediatR;

namespace AlohaMaui.Core.Commands
{
    public class CreateCommunityEventAssetsCommand : IRequest<EventAssets>
    {
        public string ImageBase64 { get; }

        public CreateCommunityEventAssetsCommand(string imageBase64) => ImageBase64 = imageBase64;
    }
}
