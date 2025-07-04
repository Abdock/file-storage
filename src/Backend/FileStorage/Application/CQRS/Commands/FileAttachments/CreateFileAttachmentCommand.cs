using System.IO.Pipelines;
using Application.Constants;
using Application.DTO.Enums;
using Application.DTO.Mapping;
using Application.DTO.Requests.FileAttachments;
using Application.DTO.Responses.FileAttachments;
using Application.DTO.Responses.General;
using Application.Extensions;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MimeTypes;
using Persistence.Context;
using Persistence.Entities;
using Persistence.Utilities;

namespace Application.CQRS.Commands.FileAttachments;

public sealed class CreateFileAttachmentCommand : ICommand<BaseResponse<FileAttachmentResponse>>
{
    public required CreateFileAttachmentRequest Request { get; init; }
}

public sealed class CreateFileAttachmentCommandHandler : ICommandHandler<CreateFileAttachmentCommand, BaseResponse<FileAttachmentResponse>>
{
    private readonly IDbContextFactory<StorageContext> _contextFactory;
    private readonly IHostEnvironment _hostEnvironment;

    public CreateFileAttachmentCommandHandler(IDbContextFactory<StorageContext> contextFactory, IHostEnvironment hostEnvironment)
    {
        _contextFactory = contextFactory;
        _hostEnvironment = hostEnvironment;
    }

    public async ValueTask<BaseResponse<FileAttachmentResponse>> Handle(CreateFileAttachmentCommand command, CancellationToken cancellationToken)
    {
        if (command.Request.Authorization.IsRevoked)
        {
            return CustomStatusCodes.ApiKeyRevoked;
        }

        if (!command.Request.Authorization.IsCanWrite())
        {
            return CustomStatusCodes.DoesNotHavePermission;
        }

        var extension = MimeTypeMap.GetExtension(command.Request.MimeType) ?? string.Empty;
        var fileName = Identifier.GenerateUlid() + extension;
        var apiKeyId = command.Request.Authorization.ApiKeyId;
        var directoryPath = Path.Combine(_hostEnvironment.ContentRootPath, ConfigurationConstants.FilesFolderName, apiKeyId.ToString("D"));
        var path = Path.Combine(directoryPath, fileName);
        if (Path.Exists(path))
        {
            return CustomStatusCodes.FileNameAlreadyUsing;
        }

        Directory.CreateDirectory(directoryPath);
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await using var fileStream = File.OpenWrite(path);
        await command.Request.Stream.CopyToAsync(PipeWriter.Create(fileStream), cancellationToken);
        var attachment = new FileAttachment
        {
            Name = fileName,
            Path = path,
            ExpiresAt = command.Request.ExpiresAt,
            CreatorApiKeyId = apiKeyId
        };
        await context.FileAttachments.AddAsync(attachment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return attachment.MapToResponse();
    }
}