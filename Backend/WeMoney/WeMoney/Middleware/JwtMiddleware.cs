using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WeMoney.Models.Constants;
using WeMoney.Services;

namespace WeMoney.Middleware;

public class JwtMiddleware(
    RequestDelegate next,
    IOptions<AppSettings> options,
    UserService userService
)
{
    private readonly string _secretKey = options.Value.SecretKey;
    
    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            AttachUserToContext(context, token);
        }

        await next(context);
    }

    private async void AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == "Id").Value;
            context.Items["User"] = userService.GetByIdAsync(userId);
        }
        catch
        {
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("Internal Server Error");
        }
    }
}