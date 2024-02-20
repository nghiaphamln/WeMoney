using System.Diagnostics;
using System.Text;
using Microsoft.IdentityModel.Tokens;

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
            if (error.IsNullOrEmpty())
            {
                logger.LogInformation("[{ProcessTime}ms] [{Status}] {RequestPath}", 
                    elapsedMilliseconds, context.Response.StatusCode, context.Request.Path);
            }
            else
            {
                logger.LogCritical("[{ProcessTime}ms] [{Status}] {RequestPath} {Error}", 
                    elapsedMilliseconds, context.Response.StatusCode, context.Request.Path, error);
            }
        }
    }
}