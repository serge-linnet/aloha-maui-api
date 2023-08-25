using AlohaMaui.Api.Models;
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Jpeg;
using Azure.Storage.Blobs;
using AlohaMaui.Core.Queries;

namespace AlohaMaui.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : BaseApiController
{
    private readonly ILogger<EventsController> _logger;
    private readonly ICommunityEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBlobServiceClientProvider _blobServiceClientProvider;

    public EventsController(ILogger<EventsController> logger, ICommunityEventRepository eventRepository, IUserRepository userRepository, IBlobServiceClientProvider blobServiceClientProvider)
    {
        _logger = logger;
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _blobServiceClientProvider = blobServiceClientProvider;
    }

    [HttpGet]
    public async Task<IEnumerable<CommunityEvent>> Find(FindPublicEventsQuery? query)
    {
        query = new FindPublicEventsQuery();
        var result = await _eventRepository.FindPublicEvents(query);
        return result;
    }

    [HttpGet("{id}/details")]
    public async Task<CommunityEvent> FindById([FromRoute] Guid id)
    {
        return await _eventRepository.Find(id);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}/status")]
    public async Task<CommunityEvent> ChangeStatus([FromRoute] Guid id, [FromBody] EventChangeStatusRequest request)
    {
        var cevent = await _eventRepository.Find(id);
        cevent.Status = request.Status;
        return await _eventRepository.Update(cevent);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("pending")]
    public IEnumerable<CommunityEvent> FindPending()
    {
        return _eventRepository.FindPendingEvents();
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> FindMyEvents()
    {
        if (!UserId.HasValue)
        {
            return Unauthorized();
        }
        var result = await _eventRepository.FindEventsForUser(UserId.Value);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] EventsRequest request)
    {
        if (!UserId.HasValue)
        {
            return Unauthorized();            
        }
        
        var newEvent = request.ToEvent();
        newEvent.Id = Guid.NewGuid();
        newEvent.UserId = UserId.Value;

        var blobClient = _blobServiceClientProvider.GetBlobClient();
        var container = blobClient.GetBlobContainerClient("event-assets");
        var containerUrl = container.Uri.ToString();

        try
        {
            var images = await ConvertAndUpload(container, request.Photo);
            newEvent.Assets = new EventAssets
            {
                CoverPhoto = $"{containerUrl}/{images.CoverPhoto}",
                Thumbnail = $"{containerUrl}/{images.Thumbnail}"
            };
        }
        catch
        {
            // no op
        }
        
        await _eventRepository.CreateEvent(newEvent);
        return Ok(newEvent);
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
            image.Save(resizedStream, new JpegEncoder() { Quality = quality });
            resizedStream.Position = 0;

            var blobName = $"{Guid.NewGuid()}.jpg";
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

