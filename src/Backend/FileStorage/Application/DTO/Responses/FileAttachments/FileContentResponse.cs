namespace Application.DTO.Responses.FileAttachments;

public sealed record FileContentResponse
{
    public required Stream Content { get; init; }
    public required string MimeType { get; init; }
}