using System.IO.Pipelines;
using Application.Constants;
using Application.DTO.Mapping;
using Application.DTO.Requests.FileAttachments;
using Application.DTO.Responses.FileAttachments;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MimeTypes;
using Persistence.Context;
using Persistence.Entities;
using Persistence.Utilities;

namespace Application.CQRS.Commands.FileAttachments;

public class CreateFileAttachmentCommand : ICommand<FileAttachmentResponse>
{
    public required CreateFileAttachmentRequest Request { get; init; }
}

public class CreateFileAttachmentCommandHandler : ICommandHandler<CreateFileAttachmentCommand, FileAttachmentResponse>
{
    private readonly IDbContextFactory<StorageContext> _contextFactory;
    private readonly IHostEnvironment _hostEnvironment;

    public CreateFileAttachmentCommandHandler(IDbContextFactory<StorageContext> contextFactory, IHostEnvironment hostEnvironment)
    {
        _contextFactory = contextFactory;
        _hostEnvironment = hostEnvironment;
    }

    public async ValueTask<FileAttachmentResponse> Handle(CreateFileAttachmentCommand command, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var extension = MimeTypeMap.GetExtension(command.Request.MimeType) ?? string.Empty;
        var name = Identifier.GenerateUlid() + extension;
        var path = Path.Combine(_hostEnvironment.ContentRootPath, ConfigurationConstants.FilesFolderName, name);
        await using var fileStream = File.OpenWrite(path);
        await command.Request.Stream.CopyToAsync(PipeWriter.Create(fileStream), cancellationToken);
        var attachment = new FileAttachment
        {
            Name = name,
            Path = path,
            ExpiresAt = command.Request.ExpiresAt,
            CreatorApiKeyId = command.Request.ApiKeyId
        };
        await context.FileAttachments.AddAsync(attachment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return attachment.MapToResponse();
    }
}