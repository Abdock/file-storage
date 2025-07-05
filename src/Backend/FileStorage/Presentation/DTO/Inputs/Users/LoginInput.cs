namespace Presentation.DTO.Inputs.Users;

public record LoginInput
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}