using AlohaMaui.Api.Models;
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Jpeg;
using Azure.Storage.Blobs;
using MediatR;
using AlohaMaui.Core.Filters;
using AlohaMaui.Core.Queries;
using AlohaMaui.Core.Commands;

namespace AlohaMaui.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : BaseApiController
{
    private readonly ILogger<EventsController> _logger;
    private readonly ICommunityEventRepository _eventRepository;
    private readonly ISender _mediator;

    public EventsController(
        ILogger<EventsController> logger, 
        ICommunityEventRepository eventRepository,
        ISender mediator)
    {
        _logger = logger;
        _eventRepository = eventRepository;    
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IEnumerable<CommunityEvent>> Find(PublicCommunityEventsFilter? query)
    {
        query = new PublicCommunityEventsFilter(); // todo: read from request

        var result = await _mediator.Send(new GetPublicCommunityEventsQuery(query));
        return result;
    }

    [HttpGet("{id}/details")]
    public async Task<CommunityEvent> FindById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetCommunityEventByIdQuery(id));
        return result;
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}/status")]
    public async Task<CommunityEvent> ChangeStatus([FromRoute] Guid id, [FromBody] EventChangeStatusRequest request)
    {
        var result = await _mediator.Send(new ChangeCommunityEventStatusCommand(id, request.Status));
        return result;
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
        var result = await _mediator.Send(new GetAllUserCommunityEventsQuery(UserId.Value));
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] EventsRequest request)
    {
        var newEvent = request.ToEvent();
        
        try
        {
            var assets = await _mediator.Send(new CreateCommunityEventAssetsCommand(request.Photo));
            newEvent.Assets = assets;
        }
        catch
        {
            // no op
        }

        await _mediator.Send(new CreateCommunityEventCommand(UserId!.Value, newEvent));
        
        return Ok(newEvent);
    }
}

