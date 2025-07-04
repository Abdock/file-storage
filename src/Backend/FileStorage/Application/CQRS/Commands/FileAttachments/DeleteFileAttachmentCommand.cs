using Application.DTO.Enums;
using Application.DTO.Requests.FileAttachments;
using Application.DTO.Responses.General;
using Application.Extensions;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.CQRS.Commands.FileAttachments;

public sealed class DeleteFileAttachmentCommand : ICommand<BaseResponse>
{
    public required DeleteFileAttachmentRequest Request { get; init; }
}

public sealed class DeleteFileAttachmentCommandHandler : ICommandHandler<DeleteFileAttachmentCommand, BaseResponse>
{
    private readonly IDbContextFactory<StorageContext> _contextFactory;

    public DeleteFileAttachmentCommandHandler(IDbContextFactory<StorageContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async ValueTask<BaseResponse> Handle(DeleteFileAttachmentCommand command, CancellationToken cancellationToken)
    {
        if (command.Request.Authorization.IsRevoked)
        {
            return CustomStatusCodes.ApiKeyRevoked;
        }

        if (!command.Request.Authorization.IsCanDelete())
        {
            return CustomStatusCodes.DoesNotHavePermission;
        }

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var affectRowsCount = await context.FileAttachments
            .Where(e => e.Id == command.Request.Id && e.CreatorApiKeyId == command.Request.Authorization.ApiKeyId)
            .ExecuteUpdateAsync(calls => calls.SetProperty(e => e.DeletedAt, DateTimeOffset.UtcNow), cancellationToken);
        return  affectRowsCount > 0 ? CustomStatusCodes.Ok : CustomStatusCodes.FileWasNotFound;
    }
}