using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WeMoney.Controllers;


[Route("api/v1/user")]
[ApiController]
public class UserController : ControllerBase
{
    [Authorize]
    [HttpGet("get")]
    public async Task<IActionResult> Get()
    {
        return Ok(new { Message = "Thành công" });
    }
}