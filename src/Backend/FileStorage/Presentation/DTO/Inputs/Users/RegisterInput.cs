namespace Presentation.DTO.Inputs.Users;

public record RegisterInput
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}