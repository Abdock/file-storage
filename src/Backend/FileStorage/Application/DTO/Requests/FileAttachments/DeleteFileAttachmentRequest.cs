namespace Application.DTO.Requests.FileAttachments;

public record DeleteFileAttachmentRequest
{
    public required Guid Id { get; init; }
}