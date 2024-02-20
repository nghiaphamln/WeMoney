using System.Diagnostics;
using System.Text;

namespace WeMoney.Middleware;

public class RequestTimingMiddleware(
    RequestDelegate next,
    ILogger<RequestTimingMiddleware> logger
)
{
    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var stringBuilder = new StringBuilder($"TimingMiddleware - Path: {context.Request.Path} ");
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
            logger.LogInformation("[{ProcessTime}ms] [{Status}] {RequestPath}", 
                elapsedMilliseconds, context.Response.StatusCode, context.Request.Path);
        }
    }
}