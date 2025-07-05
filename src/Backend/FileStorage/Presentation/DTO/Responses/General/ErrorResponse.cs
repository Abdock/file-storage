using Application.Attributes;
using Application.DTO.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace Presentation.DTO.Responses.General;

public sealed record ErrorResponse
{
    public required int HttpStatus { get; init; }
    public required CustomStatusCodes SystemStatus { get; init; }
    public required string Message { get; init; }

    public static implicit operator ErrorResponse(CustomStatusCodes statusCode)
    {
        var attribute = statusCode.GetAttributeOfType<HttpStatusCodeAttribute>();
        return new ErrorResponse
        {
            HttpStatus = attribute.Status,
            SystemStatus = statusCode,
            Message = statusCode.ToString(),
        };
    }
}