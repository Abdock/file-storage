using Persistence.Base;
using Persistence.Enums;

namespace Persistence.Entities;

public class ApiKey : BaseEntity
{
    public required Guid CreatorId { get; init; }
    public required ICollection<Permission> Permissions { get; init; }
    public bool IsRevoked { get; init; }
    public User? Creator { get; init; }
}