using Application.Attributes;
using Microsoft.AspNetCore.Http;

namespace Application.DTO.Enums;

public enum CustomStatusCodes
{
    [HttpStatusCode(StatusCodes.Status200OK)]
    Ok = 200,

    [HttpStatusCode(StatusCodes.Status400BadRequest)]
    FileNameAlreadyUsing = 400_001,
    [HttpStatusCode(StatusCodes.Status400BadRequest)]
    WeakPassword = 400_002,
    [HttpStatusCode(StatusCodes.Status400BadRequest)]
    InvalidUserCredentials = 400_003,
    [HttpStatusCode(StatusCodes.Status400BadRequest)]
    UsernameAlreadyUsing = 400_004,

    [HttpStatusCode(StatusCodes.Status403Forbidden)]
    DoesNotHavePermission = 403_001,
    [HttpStatusCode(StatusCodes.Status403Forbidden)]
    ApiKeyRevoked = 403_002,

    [HttpStatusCode(StatusCodes.Status404NotFound)]
    UserWasNotFound = 404_001,
    [HttpStatusCode(StatusCodes.Status404NotFound)]
    FileWasNotFound = 404_002,
    [HttpStatusCode(StatusCodes.Status404NotFound)]
    ApiKeyWasNotFound = 404_003,

    [HttpStatusCode(StatusCodes.Status500InternalServerError)]
    Unknown = 500_001,
}