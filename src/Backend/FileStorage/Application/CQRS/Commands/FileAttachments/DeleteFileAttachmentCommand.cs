using Application.DTO.Requests.FileAttachments;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.CQRS.Commands.FileAttachments;

public class DeleteFileAttachmentCommand : ICommand
{
    public required DeleteFileAttachmentRequest Request { get; init; }
}

public class DeleteFileAttachmentCommandHandler : ICommandHandler<DeleteFileAttachmentCommand>
{
    private readonly IDbContextFactory<StorageContext> _contextFactory;

    public DeleteFileAttachmentCommandHandler(IDbContextFactory<StorageContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async ValueTask<Unit> Handle(DeleteFileAttachmentCommand command, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await context.FileAttachments
            .Where(e => e.Id == command.Request.Id)
            .ExecuteUpdateAsync(calls => calls.SetProperty(e => e.DeletedAt, DateTimeOffset.UtcNow), cancellationToken);
        return Unit.Value;
    }
}