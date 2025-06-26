using Application.DTO.Requests.General;

namespace Application.DTO.Requests.ApiKeys;

public record RevokeApiKeyRequest : AuthorizedUserRequest
{
    public required Guid Id { get; init; }
}