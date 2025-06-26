using Application.DTO.Mapping;
using Application.DTO.Requests.General;
using Application.DTO.Responses.ApiKeys;
using Application.DTO.Responses.General;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.CQRS.Queries.ApiKeys;

public class GetUserApiKeysQuery : IQuery<BaseResponse<IReadOnlyCollection<ApiKeyResponse>>>
{
    public required AuthorizedUserRequest Request { get; init; }
}

public class GetUserApiKeysQueryHandler : IQueryHandler<GetUserApiKeysQuery, BaseResponse<IReadOnlyCollection<ApiKeyResponse>>>
{
    private readonly IDbContextFactory<StorageContext> _contextFactory;

    public GetUserApiKeysQueryHandler(IDbContextFactory<StorageContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async ValueTask<BaseResponse<IReadOnlyCollection<ApiKeyResponse>>> Handle(GetUserApiKeysQuery query, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var apiKeys = await context.ApiKeys
            .Where(e => e.CreatorId == query.Request.UserId)
            .AsNoTracking()
            .Select(ApiKeyMapping.MapToResponseQuery)
            .ToArrayAsync(cancellationToken);
        return apiKeys.AsReadOnly();
    }
}