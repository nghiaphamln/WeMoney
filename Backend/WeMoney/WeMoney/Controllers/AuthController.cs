using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using WeMoney.Models.Base;
using WeMoney.Models.Constants;
using WeMoney.Models.Entities;
using WeMoney.Models.Enums;
using WeMoney.Models.Request.Auth;
using WeMoney.Services;

namespace WeMoney.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(
    UserService userService,
    TokenService tokenService,
    PasswordHasher passwordHasher,
    IOptions<JwtSettings> jwtSettings
) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest(ModelState);
        }

        var user = await userService.GetByEmailAsync(request.Email);

        if (user is not null)
        {
            return Conflict(new BaseResponse("Email đã tồn tại"));
        }

        user = new User
        {
            FullName = request.FullName,
            Password = passwordHasher.Hash(request.Password),
            Email = request.Email,
            Role = nameof(RoleEnum.User)
        };

        await userService.CreateAsync(user);

        return Created();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest(ModelState);
        }

        var user = await userService.GetByEmailAsync(request.Email);
        if (user is null || !passwordHasher.Compare(request.Password, user.Password))
        {
            return Unauthorized(new BaseResponse("Email hoặc mật khẩu không chính xác"));
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, jwtSettings.Value.Subject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            new(ClaimType.Id, user.Id!),
            new(ClaimTypes.Role, user.Role)
        };

        var token = tokenService.GenerateAccessToken(claims);
        var refreshToken = TokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(jwtSettings.Value.JwtLifeRefreshToken);

        await userService.UpdateAsync(user);

        return Ok(new
        {
            Token = token,
            RefreshToken = refreshToken
        });
    }
}