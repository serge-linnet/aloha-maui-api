
using AlohaMaui.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace AlohaMaui.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IGoogleAuthValidator _googleAuthValidator;
    private static List<User> UserList = new List<User>()
    {
        new User() { Email = "serge@outlook.ie", Role = "Admin" }
    };

    public AuthController(ILogger<AuthController> logger, IJwtTokenGenerator jwtTokenGenerator, IGoogleAuthValidator googleAuthValidator)
    {
        _logger = logger;
        _jwtTokenGenerator = jwtTokenGenerator;
        _googleAuthValidator = googleAuthValidator;
    }

    [HttpPost("LoginWithGoogle")]
    public async Task<IActionResult> LoginWithGoogle([FromBody] string credentials)
    {
        var payload = await _googleAuthValidator.Validate(credentials);
        
        var user = UserList.Where(x => x.Email == payload.Email).FirstOrDefault();

        if (user == null)
        {
            user = new User()
            {
                Email = payload.Email,
                Role = "User",
                Name = payload.Name
            };
        }

        if (user != null)
        {
            var token = _jwtTokenGenerator.Generate(user);
            SetJwt(token.Token);

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, user);

            return Ok(user);
        }
        else
        {
            return BadRequest();
        }
    }



    private void SetJwt(string encrypterToken)
    {

        HttpContext.Response.Cookies.Append("X-Access-Token", encrypterToken,
              new CookieOptions
              {
                  Expires = DateTime.Now.AddMinutes(15),
                  HttpOnly = true,
                  Secure = true,
                  IsEssential = true,
                  SameSite = SameSiteMode.None
              });
    }

    private RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken()
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(7),
            Created = DateTime.Now
        };

        return refreshToken;
    }

    private void SetRefreshToken(RefreshToken refreshToken, User user)
    {
        HttpContext.Response.Cookies.Append("X-Refresh-Token", refreshToken.Token,
             new CookieOptions
             {
                 Expires = refreshToken.Expires,
                 HttpOnly = true,
                 Secure = true,
                 IsEssential = true,
                 SameSite = SameSiteMode.None
             });

        UserList.Where(x => x.Email == user.Email).First().Token = refreshToken.Token;
        UserList.Where(x => x.Email == user.Email).First().TokenCreated = refreshToken.Created;
        UserList.Where(x => x.Email == user.Email).First().TokenExpires = refreshToken.Expires;
    }
}

public class RefreshToken
{
    public string Token { get; set; }
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
}