using System.Diagnostics;
using System.Net;
using System.Text;
using ServiceStack;
using WeMoney.Helper;
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
        Exception? exception = null;
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            exception = e;
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            if (exception is not null)
            {
                var response = new BaseResponse(exception.Message).ToJson();
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"Exception: {exception.Message} ");
                stringBuilder.AppendLine($"StackTrace: {exception.StackTrace?.GetTrimmedStackTrace()} ");
                
                logger.LogCritical("[{ProcessTime}ms] [{Status}] {RequestPath} {Error}",
                    elapsedMilliseconds, context.Response.StatusCode, context.Request.Path, stringBuilder.ToString());
                
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(response);
            }
            else
            {
                logger.LogInformation("[{ProcessTime}ms] [{Status}] {RequestPath}",
                    elapsedMilliseconds, context.Response.StatusCode, context.Request.Path);
            }
        }
    }
}