using Application.DTO.Enums;
using Application.DTO.Mapping;
using Application.DTO.Requests.General;
using Application.DTO.Responses.General;
using Application.DTO.Responses.Users;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.CQRS.Queries.Users;

public class GetAuthorizedUserQuery : IQuery<BaseResponse<UserResponse>>
{
    public required AuthorizedUserRequest Request { get; init; }
}

public class GetAuthorizedUserQueryHandler : IQueryHandler<GetAuthorizedUserQuery, BaseResponse<UserResponse>>
{
    private readonly IDbContextFactory<StorageContext> _contextFactory;

    public GetAuthorizedUserQueryHandler(IDbContextFactory<StorageContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async ValueTask<BaseResponse<UserResponse>> Handle(GetAuthorizedUserQuery query, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Users
            .Where(e => e.Id == query.Request.UserId)
            .AsNoTracking()
            .Select(UserMapping.MapToResponseQuery)
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            return CustomStatusCodes.UserWasNotFound;
        }

        return user;
    }
}