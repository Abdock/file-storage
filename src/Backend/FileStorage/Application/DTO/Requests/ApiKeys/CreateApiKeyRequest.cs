using Application.DTO.Requests.General;
using Persistence.Enums;

namespace Application.DTO.Requests.ApiKeys;

public record CreateApiKeyRequest : AuthorizedUserRequest
{
    public required string Name { get; init; }
    public required Permission[] Permissions { get; init; }
}