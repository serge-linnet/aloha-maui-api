
using AlohaMaui.Api.Models;
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Security.Cryptography;
using System.Text;
using User = AlohaMaui.Core.Entities.User;

namespace AlohaMaui.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IGoogleAuthValidator _googleAuthValidator;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public AuthController(ILogger<AuthController> logger,
        IJwtTokenGenerator jwtTokenGenerator,
        IGoogleAuthValidator googleAuthValidator,
        IUserRepository userRepository,
        IConfiguration config)
    {
        _logger = logger;
        _jwtTokenGenerator = jwtTokenGenerator;
        _googleAuthValidator = googleAuthValidator;
        _userRepository = userRepository;
        _config = config;
    }

    [HttpPost("LoginWithGoogleRedirect")]

    public async Task<IActionResult> LoginWithGoogleRedirect([FromForm] GooggleAuthRedirectFormRequest form)
    {
        _logger.LogInformation($"LoginWithGoogleRedirect");
        var user = await AuthenticateWithGoogle(form.Credential);
        _logger.LogInformation($"LoginWithGoogleRedirect.AuthenticateWithGoogle", user.Id);
        var redirectUrl = _config.GetValue<string>("UiRedirectUrl");
        Guard.Against.NullOrWhiteSpace(redirectUrl, nameof(redirectUrl));

        var userJson = JsonConvert.SerializeObject(user, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        var userBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(userJson));

        return Redirect($"{redirectUrl}?{userBase64}");
    }

    [HttpPost("LoginWithGoogle")]
    public async Task<IActionResult> LoginWithGoogle([FromBody] string credentials)
    {
        _logger.LogInformation($"LoginWithGoogle");
        var user = await AuthenticateWithGoogle(credentials);
        _logger.LogInformation($"AuthenticateWithGoogle.AuthenticateWithGoogle", user.Id);
        return Ok(user);
    }


    [HttpPost]
    [Authorize]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete("X-Access-Token");
        HttpContext.Response.Cookies.Delete("X-Refresh-Token");

        return Ok();
    }

    [HttpGet("RefreshToken")]
    public async Task<ActionResult<string>> RefreshToken()
    {
        var refreshToken = Request.Cookies["X-Refresh-Token"];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized("No refresh token.");
        }

        var user = await _userRepository.FindByRefreshToken(refreshToken);
        if (user == null || user.TokenExpires < DateTime.Now)
        {
            return Unauthorized("Token has expired.");
        }

        await DoAllTheTokenThings(user);

        return Ok();
    }

    private void SetJwt(string encrypterToken)
    {
        HttpContext.Response.Cookies.Append("X-Access-Token", encrypterToken,
              new CookieOptions
              {
                  Expires = DateTime.Now.AddDays(7), // todo: change to 15 minutes and refresh
                  HttpOnly = true,
                  Secure = true,
                  IsEssential = true,
                  SameSite = SameSiteMode.None
              });
    }

    private async Task<User> AuthenticateWithGoogle(string credentials)
    {
        var payload = await _googleAuthValidator.Validate(credentials);

        Guard.Against.Null(payload, nameof(payload));
        Guard.Against.Null(payload.Email, nameof(payload.Email));

        var user = await _userRepository.FindByEmail(payload.Email);
        if (user == null)
        {
            user = new User(payload.Email, Role.User);
            user = await _userRepository.CreateUser(user);
        }

        await DoAllTheTokenThings(user);

        return user;
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

    private void SetRefreshToken(RefreshToken refreshToken)
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
    }

    private async Task DoAllTheTokenThings(User user)
    {
        var token = _jwtTokenGenerator.Generate(user);
        SetJwt(token.Token);

        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken);

        user.Token = refreshToken.Token;
        user.TokenCreated = refreshToken.Created;
        user.TokenExpires = refreshToken.Expires;
        await _userRepository.UpdateUser(user);
    }
}

public class RefreshToken
{
    public string Token { get; set; }
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
}