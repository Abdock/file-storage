using Presentation.Constants;
using Presentation.Extensions;
using Serilog.Context;

namespace Presentation.Middlewares;

public class EnrichLoggerContextMiddleware
{
    private readonly RequestDelegate _next;

    public EnrichLoggerContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        using var username = LogContext.PushProperty(LoggingConstants.UsernameKey, httpContext.User.GetUsername());
        using var userId = LogContext.PushProperty(LoggingConstants.UserIdKey, httpContext.User.GetUserId());
        using var correlationId = LogContext.PushProperty(LoggingConstants.CorrelationIdKey, httpContext.TraceIdentifier);
        await _next(httpContext);
    }
}