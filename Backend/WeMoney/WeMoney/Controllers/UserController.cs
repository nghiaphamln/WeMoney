using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeMoney.Models.Base;
using WeMoney.Models.Constants;
using WeMoney.Models.Entities;
using WeMoney.Services;

namespace WeMoney.Controllers;

[Route("api/v1/user")]
[ApiController]
public class UserController(UserService userService) : ControllerBase
{
    [Authorize]
    [HttpGet("get")]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirst(ClaimType.Id)!.Value;
        var userInfo = await userService.GetByIdAsync(userId);
        return Ok(new BaseResponse<User>(userInfo));
    }
}