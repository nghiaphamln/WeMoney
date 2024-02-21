using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
            Role = [nameof(RoleEnum.User)]
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

        var (token, refreshToken) = tokenService.GenerateToken(user);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(jwtSettings.Value.JwtLifeRefreshToken);

        await userService.UpdateAsync(user);

        return Ok(new
        {
            Token = token,
            RefreshToken = refreshToken
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenApiRequest request)
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest(ModelState);
        }

        var principal = tokenService.GetPrincipalFromExpiredToken(request.Token);
        var userId = principal.FindFirst(ClaimType.Id)?.Value;

        if (userId is null)
        {
            return BadRequest(new BaseResponse("Request không hợp lệ"));
        }

        var user = await userService.GetByIdAsync(userId);

        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest(new BaseResponse("Request không hợp lệ"));
        }
        
        var (token, refreshToken) = tokenService.GenerateToken(user);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(jwtSettings.Value.JwtLifeRefreshToken);

        await userService.UpdateAsync(user);

        return Ok(new
        {
            Token = token,
            RefreshToken = refreshToken
        });
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke()
    {
        var userId = User.FindFirst(ClaimType.Id)!.Value;
        var user = await userService.GetByIdAsync(userId);
        
        if (user is null)
        {
            return BadRequest();
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await userService.UpdateAsync(user);
        return NoContent();
    }
}