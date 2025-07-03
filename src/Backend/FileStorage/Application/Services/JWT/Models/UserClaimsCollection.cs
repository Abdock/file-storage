namespace Application.Services.JWT.Models;

public sealed record UserClaimsCollection
{
    public required string Id { get; init; }
    public required string Username { get; init; }
}