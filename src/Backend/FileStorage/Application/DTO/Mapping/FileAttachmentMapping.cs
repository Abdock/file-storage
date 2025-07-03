using System.Linq.Expressions;
using Application.DTO.Responses.FileAttachments;
using Persistence.Entities;

namespace Application.DTO.Mapping;

public static class FileAttachmentMapping
{
    public static readonly Expression<Func<FileAttachment, FileAttachmentResponse>> MapToResponseQuery = file => new FileAttachmentResponse
    {
        Id = file.Id,
        Name = file.Name
    };

    public static FileAttachmentResponse MapToResponse(this FileAttachment attachment) => new()
    {
        Id = attachment.Id,
        Name = attachment.Name
    };
}