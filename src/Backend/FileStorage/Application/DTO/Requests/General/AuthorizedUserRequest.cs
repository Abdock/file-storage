namespace Application.DTO.Requests.General;

public record AuthorizedUserRequest
{
    public required Guid UserId { get; init; }
}