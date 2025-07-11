﻿using Application.DTO.Enums;
using Application.DTO.Requests.FileAttachments;
using Application.DTO.Responses.FileAttachments;
using Application.DTO.Responses.General;
using Application.Extensions;
using Mediator;
using Microsoft.EntityFrameworkCore;
using MimeTypes;
using Persistence.Context;

namespace Application.CQRS.Queries.FileAttachments;

public class GetFileAttachmentQuery : IQuery<BaseResponse<FileContentResponse>>
{
    public required GetFileAttachmentRequest Request { get; init; }
}

public class GetFileAttachmentQueryHandler : IQueryHandler<GetFileAttachmentQuery, BaseResponse<FileContentResponse>>
{
    private readonly IDbContextFactory<StorageContext> _contextFactory;

    public GetFileAttachmentQueryHandler(IDbContextFactory<StorageContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async ValueTask<BaseResponse<FileContentResponse>> Handle(GetFileAttachmentQuery query, CancellationToken cancellationToken)
    {
        if (query.Request.Authorization.IsRevoked)
        {
            return CustomStatusCodes.ApiKeyRevoked;
        }

        if (!query.Request.Authorization.IsCanRead())
        {
            return CustomStatusCodes.DoesNotHavePermission;
        }

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var path = await context.FileAttachments
            .Where(e => e.Name == query.Request.FileName)
            .AsNoTracking()
            .Select(e => e.Path)
            .FirstOrDefaultAsync(cancellationToken);
        if (!Path.Exists(path))
        {
            return CustomStatusCodes.FileWasNotFound;
        }

        return new FileContentResponse
        {
            Content = File.OpenRead(path),
            MimeType = MimeTypeMap.GetMimeType(Path.GetFileName(path))
        };
    }
}