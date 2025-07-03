using Persistence.Base;

namespace Persistence.Entities;

public sealed class FileAttachment : BaseEntity
{
    public required string Name { get; init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Path { get; init; }
    public required DateTimeOffset? ExpiresAt { get; init; }
    public required Guid CreatorApiKeyId { get; init; }
    public ApiKey? CreatorApiKey { get; init; }
}