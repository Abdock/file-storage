namespace Application.DTO.Requests.General;

public record AuthorizedApiRequest
{
    public required Guid ApiKeyId { get; init; }
}