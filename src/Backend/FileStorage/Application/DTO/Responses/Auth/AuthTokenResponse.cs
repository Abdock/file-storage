namespace Application.DTO.Responses.Auth;

public sealed record AuthTokenResponse
{
    public required string AccessToken { get; init; }
}