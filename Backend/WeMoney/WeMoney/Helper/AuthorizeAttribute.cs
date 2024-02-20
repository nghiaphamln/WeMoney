using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WeMoney.Models.Entities;

namespace WeMoney.Helper;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (User?)context.HttpContext.Items["User"];
        if (user is null)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}