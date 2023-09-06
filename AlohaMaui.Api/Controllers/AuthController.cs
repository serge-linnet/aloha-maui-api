
using AlohaMaui.Api.Models;
using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using User = AlohaMaui.Core.Entities.User;

namespace AlohaMaui.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _hasher;

    public AuthController(ILogger<AuthController> logger,
        IJwtTokenGenerator jwtTokenGenerator,
        IUserRepository userRepository,
        IPasswordHasher hasher)
    {
        _logger = logger;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
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
                
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user);
        user.Token = refreshToken.Token;
        user.TokenExpires = refreshToken.Expires;
        await _userRepository.UpdateUser(user);

        _logger.LogInformation($"SignIn:Success", model.Email);

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken.Token });
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

        user = new User(model.Email, Role.User)
        {
            PasswordHash = _hasher.HashPassword(model.Email, model.Password),
            Created = DateTime.Now,
            Updated = DateTime.Now,
            Id = Guid.NewGuid(),
        };

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user);
        user.Token = refreshToken.Token;
        user.TokenExpires = refreshToken.Expires;

        await _userRepository.CreateUser(user);

        _logger.LogInformation($"SignUp.Success", model.Email);

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken.Token });
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refresh)
    {
        var token = refresh.Token;
        _logger.LogInformation($"Refresh", token);
        var user = await _userRepository.FindByRefreshToken(token);

        if (user == null)
        {
            _logger.LogInformation($"Refresh:Fail-NotExist", token);
            return Unauthorized("User with this refresh token does not exist.");
        }

        if (user.TokenExpires <= DateTime.UtcNow)
        {
            _logger.LogInformation($"Refresh:Fail-Expired", token);
            return Unauthorized("Refresh token has expired.");
        }

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user);
        user.Token = refreshToken.Token;
        user.TokenExpires = refreshToken.Expires;

        await _userRepository.UpdateUser(user);

        _logger.LogInformation($"Refresh.Success", user.Email);

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken.Token });
    }

    [HttpPost]
    [Authorize]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete("X-Access-Token");
        HttpContext.Response.Cookies.Delete("X-Refresh-Token");

        return Ok();
    }
}
