using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AlohaMaui.Api.Controllers;

public class BaseApiController : ControllerBase
{
    protected Guid? UserId
    {
        get
        {
            var user = User?.Identity as ClaimsIdentity;
            var id = user?.FindFirst("id")?.Value;

            if (id == null)
            {
                return null;
            }
            return Guid.Parse(id);
        }
    }
}

