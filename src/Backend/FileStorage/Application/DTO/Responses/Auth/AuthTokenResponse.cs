namespace Application.DTO.Responses.Auth;

public record AuthTokenResponse
{
    public required string AccessToken { get; init; }
}