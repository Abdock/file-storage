using Application.DTO.Enums;
using Application.DTO.Requests.ApiKeys;
using Application.DTO.Responses.General;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.CQRS.Commands.ApiKeys;

public sealed class RevokeApiKeyCommand : ICommand<BaseResponse>
{
    public required RevokeApiKeyRequest Request { get; init; }
}

public sealed class RevokeApiKeyCommandHandler : ICommandHandler<RevokeApiKeyCommand, BaseResponse>
{
    private readonly IDbContextFactory<StorageContext> _contextFactory;

    public RevokeApiKeyCommandHandler(IDbContextFactory<StorageContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async ValueTask<BaseResponse> Handle(RevokeApiKeyCommand command, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var affectedRowsCount = await context.ApiKeys
            .Where(e => e.Id == command.Request.Id && e.CreatorId == command.Request.UserId)
            .ExecuteUpdateAsync(calls => calls.SetProperty(e => e.IsRevoked, true), cancellationToken);
        return affectedRowsCount > 0 ? CustomStatusCodes.Ok : CustomStatusCodes.ApiKeyWasNotFound;
    }
}