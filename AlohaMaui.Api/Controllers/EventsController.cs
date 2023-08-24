using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlohaMaui.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    private readonly ILogger<EventsController> _logger;
    private readonly IEventRepository _eventRepository;

    public EventsController(ILogger<EventsController> logger, IEventRepository eventRepository)
    {
        _logger = logger;
        _eventRepository = eventRepository;
    }

    [HttpGet]
    public IEnumerable<Event> Find(string? query = "")
    {
        var result = _eventRepository.FindEvents(query);
        return result;
    }

    [HttpGet("{id}/details")]
    public async Task<Event> FindById([FromRoute] Guid id)
    {
        return await _eventRepository.Find(id);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("pending")]
    public IEnumerable<Event> FindPending()
    {
        return _eventRepository.FindPendingEvents();
    }
}

