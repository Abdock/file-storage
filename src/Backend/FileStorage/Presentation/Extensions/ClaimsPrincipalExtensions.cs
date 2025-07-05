using System.Security.Claims;
using Application.Constants;
using Application.DTO.Requests.General;

namespace Presentation.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
    }

    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(CustomClaimTypes.UserId);
        return value is null ? Guid.Empty : Guid.Parse(value);
    }

    public static AuthorizedUserRequest GetAuthorizedUser(this ClaimsPrincipal user)
    {
        return new AuthorizedUserRequest
        {
            UserId = user.GetUserId()
        };
    }
}