using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeMoney.Models.Base;

namespace WeMoney.Controllers;

[ApiController]
[Route("/")]
public class HomeController : ControllerBase
{
    [HttpGet("")]
    [AllowAnonymous]
    public ActionResult Index()
    {
        return Ok(new BaseResponse("WeMoney API!"));
    }
}