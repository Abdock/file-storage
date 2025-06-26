using Application.DTO.Requests.General;

namespace Application.DTO.Requests.FileAttachments;

public record DeleteFileAttachmentRequest : AuthorizedApiRequest
{
    public required Guid Id { get; init; }
}