using Application.DTO.Mapping;
using Application.DTO.Requests.ApiKeys;
using Application.DTO.Responses.ApiKeys;
using Application.DTO.Responses.General;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Entities;

namespace Application.CQRS.Commands.ApiKeys;

public sealed class CreateApiKeyCommand : ICommand<BaseResponse<ApiKeyResponse>>
{
    public required CreateApiKeyRequest Request { get; init; }
}

public class CreateApiKeyCommandHandler : ICommandHandler<CreateApiKeyCommand, BaseResponse<ApiKeyResponse>>
{
    private readonly IDbContextFactory<StorageContext> _contextFactory;

    public CreateApiKeyCommandHandler(IDbContextFactory<StorageContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async ValueTask<BaseResponse<ApiKeyResponse>> Handle(CreateApiKeyCommand command, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var token = Guid.NewGuid().ToString();
        var apiKey = new ApiKey
        {
            CreatorId = command.Request.UserId,
            Name = command.Request.Name,
            Token = token,
            Permissions = command.Request.Permissions
        };
        await context.ApiKeys.AddAsync(apiKey, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return apiKey.MapToResponse();
    }
}