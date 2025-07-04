using Application.DTO.Requests.General;

namespace Application.DTO.Requests.FileAttachments;

public sealed record DeleteFileAttachmentRequest
{
    public required Guid Id { get; init; }
    public required AuthorizedApiRequest Authorization { get; init; }
}