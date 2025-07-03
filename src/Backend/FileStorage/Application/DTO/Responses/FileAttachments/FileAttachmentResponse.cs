namespace Application.DTO.Responses.FileAttachments;

public sealed record FileAttachmentResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}