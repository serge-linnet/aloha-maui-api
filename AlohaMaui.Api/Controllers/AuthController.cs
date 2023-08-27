
using AlohaMaui.Api.Models;
using AlohaMaui.Core.Repositories;
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
    private readonly IConfiguration _config;
    private readonly IPasswordHasher _hasher;

    public AuthController(ILogger<AuthController> logger,
        IJwtTokenGenerator jwtTokenGenerator,
        IGoogleAuthValidator googleAuthValidator,
        IUserRepository userRepository,
        IConfiguration config,
        IPasswordHasher hasher)
    {
        _logger = logger;
        _jwtTokenGenerator = jwtTokenGenerator;
        _googleAuthValidator = googleAuthValidator;
        _userRepository = userRepository;
        _config = config;
        _hasher = hasher;
    }

    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest model)
    {
        _logger.LogInformation($"SignIn");
        var user = await _userRepository.FindByEmail(model.Email);

        if (user == null || _hasher.HashPassword(model.Email, model.Password) != user.PasswordHash)
        {
            _logger.LogInformation($"SignIn:Fail-NotExists", model.Email);
            return Unauthorized("User with this email and password does not exist.");
        }

        var token = _jwtTokenGenerator.Generate(user);
        SetJwt(token.Token);

        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken);

        user.Token = refreshToken.Token;
        user.TokenCreated = refreshToken.Created;
        user.TokenExpires = refreshToken.Expires;

        await _userRepository.UpdateUser(user);

        _logger.LogInformation($"SignIn:Success", model.Email);

        return Ok(new UserInfo(user));
    }

    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest model)
    {
        _logger.LogInformation($"SignUp", model.Email);
        var user = await _userRepository.FindByEmail(model.Email);

        if (user != null)
        {
            _logger.LogInformation($"SignUp:Fail-AlreadyExists", model.Email);
            return Conflict("User with this email and password already exist.");
        }

        user = new User()
        {
            Email = model.Email,
            PasswordHash = _hasher.HashPassword(model.Email, model.Password),
            Created = DateTime.Now,
            Updated = DateTime.Now,
            Id = Guid.NewGuid(),
            Role = "User"
        };

        var token = _jwtTokenGenerator.Generate(user);
        SetJwt(token.Token);

        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken);

        user.Token = refreshToken.Token;
        user.TokenCreated = refreshToken.Created;
        user.TokenExpires = refreshToken.Expires;

        await _userRepository.CreateUser(user);

        _logger.LogInformation($"SignUp.Success", model.Email);

        return Ok(new UserInfo(user));
    }

    [HttpPost]
    [Authorize]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete("X-Access-Token");
        HttpContext.Response.Cookies.Delete("X-Refresh-Token");

        return Ok();
    }

    //[HttpGet("RefreshToken")]
    //public async Task<ActionResult<string>> RefreshToken()
    //{
    //    var refreshToken = Request.Cookies["X-Refresh-Token"];
    //    if (string.IsNullOrWhiteSpace(refreshToken))
    //    {
    //        return Unauthorized("No refresh token.");
    //    }

    //    var user = await _userRepository.FindByRefreshToken(refreshToken);
    //    if (user == null || user.TokenExpires < DateTime.Now)
    //    {
    //        return Unauthorized("Token has expired.");
    //    }

    //    await DoAllTheTokenThings(user);

    //    return Ok();
    //}

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
