using Application.Attributes;
using Application.DTO.Enums;
using Application.DTO.Responses.General;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using Presentation.DTO.Responses.General;

namespace Presentation.Extensions;

public static class MappingExtensions
{
    public static IActionResult AsErrorActionResult(this CustomStatusCodes statusCode)
    {
        ErrorResponse error = statusCode;
        return new ObjectResult(error)
        {
            StatusCode = error.HttpStatus
        };
    }

    public static IActionResult AsErrorActionResult<TResponse>(this BaseResponse<TResponse> response)
    {
        ErrorResponse error = response.StatusCode;
        return new ObjectResult(error)
        {
            StatusCode = error.HttpStatus
        };
    }

    public static IActionResult AsActionResult<TResponse>(this BaseResponse<TResponse> response)
    {
        return new ObjectResult(response)
        {
            StatusCode = response.StatusCode.GetAttributeOfType<HttpStatusCodeAttribute>().Status
        };
    }
}