using AlohaMaui.Api.Models;
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Azure.Storage.Blobs.Models;

namespace AlohaMaui.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : BaseApiController
{
    private readonly ILogger<HelloController> _logger;
    private readonly IConfiguration _configuration;

    public HelloController(ILogger<HelloController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Get()
    {        
        return Ok(_configuration["AllowedHosts"]);
    }
}

