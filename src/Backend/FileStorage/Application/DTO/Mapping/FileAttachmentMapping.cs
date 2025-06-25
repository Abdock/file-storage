using Application.DTO.Responses.FileAttachments;
using Persistence.Entities;

namespace Application.DTO.Mapping;

public static class FileAttachmentMapping
{
    public static FileAttachmentResponse MapToResponse(this FileAttachment attachment) => new()
    {
        Id = attachment.Id,
        Name = attachment.Name
    };
}