namespace Application.DTO.Responses.Users;

public sealed record UserResponse
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
}