using Application.DTO.Enums;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTO.Responses.General;

namespace Presentation.Extensions;

public static class CustomStatusCodesExtensions
{
    public static IActionResult MapToActionResult(this CustomStatusCodes statusCode)
    {
        ErrorResponse error = statusCode;
        return new ObjectResult(error)
        {
            StatusCode = error.HttpStatus
        };
    }
}