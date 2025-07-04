using Application.DTO.Requests.General;

namespace Application.DTO.Requests.FileAttachments;

public sealed record CreateFileAttachmentRequest
{
    public required Stream Stream { get; init; }
    public required string MimeType { get; init; }
    public required DateTimeOffset? ExpiresAt { get; init; }
    public required AuthorizedApiRequest Authorization { get; init; }
}