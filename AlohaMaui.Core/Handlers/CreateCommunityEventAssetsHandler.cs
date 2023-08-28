using AlohaMaui.Core.Commands;
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Providers;
using Azure.Storage.Blobs;
using MediatR;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace AlohaMaui.Core.Handlers
{
    public class CreateCommunityEventAssetsHandler : IRequestHandler<CreateCommunityEventAssetsCommand, EventAssets>
    {
        private readonly IBlobServiceClientProvider _blobServiceClientProvider;

        public CreateCommunityEventAssetsHandler(IBlobServiceClientProvider provider) => _blobServiceClientProvider = provider;

        public async Task<EventAssets> Handle(CreateCommunityEventAssetsCommand request, CancellationToken cancellationToken)
        {
            var blobClient = _blobServiceClientProvider.GetBlobClient();
            var container = blobClient.GetBlobContainerClient("event-assets");
            var containerUrl = container.Uri.ToString();

            var images = await ConvertAndUpload(container, request.ImageBase64);

            var assets = new EventAssets
            {
                CoverPhoto = $"{containerUrl}/{images.CoverPhoto}",
                Thumbnail = $"{containerUrl}/{images.Thumbnail}"
            };
            return assets;
        }

        private async Task<EventImages> ConvertAndUpload(BlobContainerClient container, string dataUrl)
        {
            var parts = dataUrl.Split(',');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid data URL format.");
            }

            var base64Data = dataUrl.Split(',')[1];
            var imageBytes = Convert.FromBase64String(base64Data);

            using (var stream = new MemoryStream(imageBytes))
            {
                using (var image = Image.Load(stream))
                {                    
                    var cover = image.Clone(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(Math.Min(1600, image.Width), Math.Min(image.Height, 800)),
                        Mode = ResizeMode.Max
                    }));
                    var coverPhoto = await UploadToBlobStorage(container, cover);

                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(Math.Min(600, image.Width), Math.Min(image.Height, 300)),
                        Mode = ResizeMode.Max
                    }));
                    var thumbnail = await UploadToBlobStorage(container, image, 70);

                    return new EventImages
                    {
                        Thumbnail = thumbnail,
                        CoverPhoto = coverPhoto
                    };
                }
            }
        }

        private async Task<string> UploadToBlobStorage(BlobContainerClient container, Image image, int quality = 80)
        {
            using (var resizedStream = new MemoryStream())
            {
                ImageEncoder encoder;
                string extention = "jpg";

                if (image.Metadata.DecodedImageFormat is PngFormat)
                {
                    encoder = new PngEncoder();
                    extention = "png";
                }
                else
                {
                    encoder = new JpegEncoder() { Quality = quality };
                }

                image.Save(resizedStream, encoder);
                resizedStream.Position = 0;

                var blobName = $"{Guid.NewGuid()}.${extention}";
                await container.UploadBlobAsync(blobName, resizedStream);

                return blobName;
            }
        }

        private struct EventImages
        {
            public string? CoverPhoto { get; set; }
            public string? Thumbnail { get; set; }
        }
    }


}
