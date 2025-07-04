using Application.DTO.Enums;
using Application.DTO.Mapping;
using Application.DTO.Requests.FileAttachments;
using Application.DTO.Responses.FileAttachments;
using Application.DTO.Responses.General;
using Application.Extensions;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.CQRS.Queries.FileAttachments;

public class GetUserFileAttachmentsQuery : IQuery<BaseResponse<PagedResponse<FileAttachmentResponse>>>
{
    public required GetUserFileAttachmentsRequest Request { get; init; }
}

public class GetUserFilesQueryHandler : IQueryHandler<GetUserFileAttachmentsQuery, BaseResponse<PagedResponse<FileAttachmentResponse>>>
{
    private readonly IDbContextFactory<StorageContext> _contextFactory;

    public GetUserFilesQueryHandler(IDbContextFactory<StorageContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async ValueTask<BaseResponse<PagedResponse<FileAttachmentResponse>>> Handle(GetUserFileAttachmentsQuery query, CancellationToken cancellationToken)
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
        var response = await context.FileAttachments
            .Where(e => e.CreatorApiKeyId == query.Request.Authorization.ApiKeyId)
            .AsNoTracking()
            .Select(FileAttachmentMapping.MapToResponseQuery)
            .ApplyPaginationAsync(query.Request, cancellationToken);
        return response;
    }
}