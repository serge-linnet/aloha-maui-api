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
using System.Reflection;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace AlohaMaui.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : BaseApiController
{
    private readonly ILogger<EventsController> _logger;
    private readonly ICommunityEventRepository _eventRepository;
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public EventsController(
        ILogger<EventsController> logger, 
        ICommunityEventRepository eventRepository,
        ISender mediator,
        IMapper mapper)
    {
        _logger = logger;
        _eventRepository = eventRepository;    
        _mediator = mediator;
        _mapper = mapper;
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
    [HttpGet("manage")]
    public async Task<IEnumerable<CommunityEvent>> FindForAdmin([FromQuery] ManageCommunityEventsFilter filter)
    {
        var result = await _mediator.Send(new GetCommunityEventsForAdminQuery(filter));
        return result;
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
        catch(Exception ex)
        {
            _logger.LogError(ex, $"CreateEvent:CreateAssets:Fail(UserId={UserId}; EventId={newEvent.Id}");
            // no op
        }

        await _mediator.Send(new CreateCommunityEventCommand(UserId!.Value, newEvent));
        _logger.LogInformation($"CreateEvent:Success(UserId={UserId}; EventId={newEvent.Id})");

        return Ok(newEvent);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] UpdateCommunityEventRequest request)
    {
        var oldEvent = await _mediator.Send(new GetCommunityEventByIdQuery(request.Id));
        if (oldEvent == null)
        {
            return NotFound();
        }

        if (UserRole == Role.User && oldEvent.UserId != UserId)
        {
            return Unauthorized();
        }

        var evnt = _mapper.Map<CommunityEvent>(request);
        evnt.UserId = oldEvent.UserId;

        if (!string.IsNullOrEmpty(request.Photo))
        {
            try
            {
                var assets = await _mediator.Send(new CreateCommunityEventAssetsCommand(request.Photo));
                evnt.Assets = assets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdateEvent:CreateAssets:Fail(UserId={UserId}; EventId={evnt.Id}");
                // no op
            }
        } 
        else
        {
            evnt.Assets = oldEvent.Assets;
        }

        await _mediator.Send(new UpdateCommunityEventCommand(evnt));
        _logger.LogInformation($"UpdateEvent:Success(UserId={UserId}; EventId={evnt.Id})");

        return Ok(evnt);

        throw new NotImplementedException();
    }
}

