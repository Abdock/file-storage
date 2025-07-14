using Application.DTO.Enums;
using Presentation.DTO.Responses.General;

namespace Presentation.Middlewares;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public CustomExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unknown error during execute request");
            ErrorResponse error = CustomStatusCodes.Unknown;
            context.Response.StatusCode = error.HttpStatus;
            await context.Response.WriteAsJsonAsync(error);
        }
    }
}