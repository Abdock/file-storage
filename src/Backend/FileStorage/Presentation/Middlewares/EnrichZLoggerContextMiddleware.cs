using Presentation.Constants;
using Presentation.Extensions;

namespace Presentation.Middlewares;

public class EnrichZLoggerContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<EnrichZLoggerContextMiddleware> _logger;

    public EnrichZLoggerContextMiddleware(RequestDelegate next, ILogger<EnrichZLoggerContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var isHaveAuthorizationHeader = httpContext.Request.Headers.ContainsKey(HttpHeaderConstants.Authorization);
        var isHaveApiKeyHeader = httpContext.Request.Headers.ContainsKey(HttpHeaderConstants.ApiKey);
        const string noHeaderValue = "unknown";
        var apiKeyValue = isHaveApiKeyHeader ? httpContext.GetApiKey() : noHeaderValue;
        var userIdValue = isHaveAuthorizationHeader ? httpContext.User.GetUserId().ToString() : noHeaderValue;
        var props = new List<KeyValuePair<string, object>>
        {
            new(LoggingConstants.CorrelationIdKey, httpContext.TraceIdentifier),
            new(LoggingConstants.ApiTokenKey, apiKeyValue),
            new(LoggingConstants.UserIdKey, userIdValue)
        };
        using var scope = _logger.BeginScope(props);
        await _next(httpContext);
    }
}