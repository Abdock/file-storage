using Application.DTO.Requests.General;

namespace Application.DTO.Requests.ApiKeys;

public sealed record RevokeApiKeyRequest
{
    public required Guid Id { get; init; }
    public required AuthorizedUserRequest Authorization { get; init; }
}