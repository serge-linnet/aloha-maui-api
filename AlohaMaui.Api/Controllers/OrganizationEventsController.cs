using AlohaMaui.Api.Models;
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Jpeg;
using Azure.Storage.Blobs;
using MediatR;
using AlohaMaui.Core.Queries;
using AlohaMaui.Core.Commands;
using AutoMapper;
using LazyCache;

namespace AlohaMaui.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganizationEventsController : BaseApiController
{
    private readonly ILogger<OrganizationEventsController> _logger;
    private readonly ISender _mediator;
    private readonly IMapper _mapper;
    private readonly IAppCache _cache;

    public OrganizationEventsController(
        ILogger<OrganizationEventsController> logger, 
        ISender mediator,
        IMapper mapper,
        IAppCache cache)
    {
        _logger = logger;
        _mediator = mediator;
        _mapper = mapper;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IEnumerable<OrganizationEvent>> Find()
    {
        var query = new GetAllOrgEventsQuery();
        var result = await _cache.GetOrAddAsync(query.GetCacheKey(), () => _mediator.Send(query), DateTime.UtcNow.AddHours(4));

        return result;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrganizationEventModel request)
    {
        var pledge = _mapper.Map<OrganizationEvent>(request);
        var newPledge = await _mediator.Send(new CreateOrgEventCommand(pledge));
        _logger.LogInformation($"CreatePledge:Success(EventId={newPledge.Id})");

        _cache.Remove(new GetAllOrgEventsQuery().GetCacheKey());
        _cache.Remove(new GetOrgEventTotalsQuery().GetCacheKey());

        return Ok(newPledge);
    }

    [HttpGet]
    [Route("totals")]
    public async Task<IActionResult> GetTotals()
    {
        var query = new GetOrgEventTotalsQuery();
        var result = await _cache.GetOrAddAsync(query.GetCacheKey(), () => _mediator.Send(query), DateTime.UtcNow.AddHours(4));

        return Ok(result);
    }
}

