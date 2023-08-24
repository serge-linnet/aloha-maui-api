
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
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

    public AuthController(ILogger<AuthController> logger, IJwtTokenGenerator jwtTokenGenerator, IGoogleAuthValidator googleAuthValidator, IUserRepository userRepository)
    {
        _logger = logger;
        _jwtTokenGenerator = jwtTokenGenerator;
        _googleAuthValidator = googleAuthValidator;
        _userRepository = userRepository;
    }

    [HttpPost("LoginWithGoogle")]
    public async Task<IActionResult> LoginWithGoogle([FromBody] string credentials)
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

        var token = _jwtTokenGenerator.Generate(user);
        SetJwt(token.Token);

        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken);

        user.Token = refreshToken.Token;
        user.TokenCreated = refreshToken.Created;
        user.TokenExpires = refreshToken.Expires;
        await _userRepository.UpdateUser(user);

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
}

public class RefreshToken
{
    public string Token { get; set; }
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
}