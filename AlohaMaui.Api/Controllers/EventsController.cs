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
using LazyCache;

namespace AlohaMaui.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : BaseApiController
{
    private readonly ILogger<EventsController> _logger;
    private readonly ICommunityEventRepository _eventRepository;
    private readonly ISender _mediator;
    private readonly IMapper _mapper;
    private readonly IAppCache _cache;

    public EventsController(
        ILogger<EventsController> logger,
        ICommunityEventRepository eventRepository,
        ISender mediator,
        IMapper mapper,
        IAppCache cache)
    {
        _logger = logger;
        _eventRepository = eventRepository;
        _mediator = mediator;
        _mapper = mapper;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IEnumerable<CommunityEvent>> Find([FromQuery] string? country, [FromQuery] bool? isOffline)
    {
        var filter = new PublicCommunityEventsFilter()
        {
            Country = country,
            IsOffline = isOffline
        };

        var query = new GetPublicCommunityEventsQuery(filter);

        IEnumerable<CommunityEvent> result;
        if (filter == new PublicCommunityEventsFilter())
        {
            result = await _cache.GetOrAddAsync(query.GetCacheKey(), () => _mediator.Send(query),
                              DateTime.UtcNow.AddHours(1));
        }
        else
        {
            result = await _mediator.Send(query);
        }

        return result;
    }

    [HttpGet("countries")]
    public async Task<IEnumerable<string>> FindAllCountries()
    {
        var query = new GetAllEventCountriesQuery();
        var result = await _cache.GetOrAddAsync(query.GetCacheKey(), () => _mediator.Send(query),
            DateTime.UtcNow.AddHours(1));

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
        _cache.Remove(new GetPublicCommunityEventsQuery(new PublicCommunityEventsFilter()).GetCacheKey());
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

        var query = new GetAllUserCommunityEventsQuery(UserId.Value);
        var result = await _cache.GetOrAddAsync(query.GetCacheKey(), () => _mediator.Send(query));

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
        catch (Exception ex)
        {
            _logger.LogError(ex, $"CreateEvent:CreateAssets:Fail(UserId={UserId}; EventId={newEvent.Id}");
            // no op
        }

        await _mediator.Send(new CreateCommunityEventCommand(UserId!.Value, newEvent));
        _logger.LogInformation($"CreateEvent:Success(UserId={UserId}; EventId={newEvent.Id})");

        _cache.Remove(new GetAllUserCommunityEventsQuery(UserId.Value).GetCacheKey());
        _cache.Remove(new GetPublicCommunityEventsQuery(new PublicCommunityEventsFilter()).GetCacheKey());

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

        _cache.Remove(new GetPublicCommunityEventsQuery(new PublicCommunityEventsFilter()).GetCacheKey());

        return Ok(evnt);
    }
}

