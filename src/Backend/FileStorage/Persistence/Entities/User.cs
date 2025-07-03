using Persistence.Base;

namespace Persistence.Entities;

public sealed class User : BaseEntity
{
    public required string Username { get; init; }
    public required string PasswordHash { get; init; }
}