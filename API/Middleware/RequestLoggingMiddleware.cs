
using System.Diagnostics;

namespace API.Middleware;

public class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation($"Request received: {context.Request.Method} {context.Request.Path} {context.Response.StatusCode} за {stopwatch.ElapsedMilliseconds} мс");
        await _next(context);
        _logger.LogInformation($"Response sent: {context.Request.Method} {context.Request.Path} {context.Response.StatusCode} за {stopwatch.ElapsedMilliseconds} мс");
    }
}
