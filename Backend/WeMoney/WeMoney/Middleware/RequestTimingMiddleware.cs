using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ServiceStack;
using WeMoney.Models.Base;

namespace WeMoney.Middleware;

public class RequestTimingMiddleware(
    RequestDelegate next,
    ILogger<RequestTimingMiddleware> logger
)
{
    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var stringBuilder = new StringBuilder();
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            stringBuilder.Append($"Exception: {e.Message} ");
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            var error = stringBuilder.ToString();
            if (CollectionUtilities.IsNullOrEmpty(error))
            {
                logger.LogInformation("[{ProcessTime}ms] [{Status}] {RequestPath}", 
                    elapsedMilliseconds, context.Response.StatusCode, context.Request.Path);
            }
            else
            {
                logger.LogCritical("[{ProcessTime}ms] [{Status}] {RequestPath} {Error}", 
                    elapsedMilliseconds, context.Response.StatusCode, context.Request.Path, error);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var response = new BaseResponse(error).ToJson();
                await context.Response.WriteAsync(response);
            }
        }
    }
}