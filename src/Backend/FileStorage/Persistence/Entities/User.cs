using Persistence.Base;

namespace Persistence.Entities;

public class User : BaseEntity
{
    public required string Username { get; init; }
    public required string PasswordHash { get; init; }
}