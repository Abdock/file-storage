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
        var isHaveAuthorizationHeader = httpContext.Request.Headers.ContainsKey(HttpHeaderConstants.Authorization);
        var isHaveApiKeyHeader = httpContext.Request.Headers.ContainsKey(HttpHeaderConstants.ApiKey);
        const string noHeaderValue = "unknown";
        var apiKeyValue = isHaveApiKeyHeader ? httpContext.GetApiKey() : noHeaderValue;
        var userIdValue = isHaveAuthorizationHeader ? httpContext.User.GetUserId().ToString() : noHeaderValue;
        using var correlationId = LogContext.PushProperty(LoggingConstants.CorrelationIdKey, httpContext.TraceIdentifier);
        using var apiKey = LogContext.PushProperty(LoggingConstants.ApiTokenKey, apiKeyValue);
        using var userId = LogContext.PushProperty(LoggingConstants.UserIdKey, userIdValue);
        await _next(httpContext);
    }
}