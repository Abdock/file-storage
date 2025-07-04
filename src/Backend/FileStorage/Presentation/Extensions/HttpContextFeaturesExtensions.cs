using Application.DTO.Requests.General;
using Persistence.Entities;
using Persistence.Enums;

namespace Presentation.Extensions;

public static class HttpContextFeaturesExtensions
{
    public static AuthorizedApiRequest? GetAuthorizedRequestOrDefault(this HttpContext httpContext)
    {
        var key = httpContext.Features.Get<ApiKey>();
        if (key is null)
        {
            return null;
        }

        return new AuthorizedApiRequest
        {
            ApiKeyId = key.Id,
            Permissions = key.Permissions.ToArray(),
            IsRevoked = key.IsRevoked
        };
    }
}