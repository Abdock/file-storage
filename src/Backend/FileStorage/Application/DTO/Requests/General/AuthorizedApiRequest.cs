using Persistence.Enums;

namespace Application.DTO.Requests.General;

public record AuthorizedApiRequest
{
    public required Guid ApiKeyId { get; init; }
    public required Permission[] Permissions { get; init; }
    public required bool IsRevoked { get; init; }
}