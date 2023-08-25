using Microsoft.AspNetCore.Mvc;

namespace AlohaMaui.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : BaseApiController
{
    public HelloController()
    {
    }

    [HttpGet]
    public IActionResult Get()
    {        
        return Ok();
    }
}

