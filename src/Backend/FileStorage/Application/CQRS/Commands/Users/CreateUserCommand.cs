using Application.DTO.Enums;
using Application.DTO.Mapping;
using Application.DTO.Requests.Users;
using Application.DTO.Responses.General;
using Application.DTO.Responses.Users;
using Application.Extensions;
using Application.Services.Hashing;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Entities;

namespace Application.CQRS.Commands.Users;

public class CreateUserCommand : ICommand<BaseResponse<UserResponse>>
{
    public required CreateUserRequest Request { get; init; }
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, BaseResponse<UserResponse>>
{
    private readonly IHasher _hasher;
    private readonly IDbContextFactory<StorageContext> _contextFactory;

    public CreateUserCommandHandler(IHasher hasher, IDbContextFactory<StorageContext> contextFactory)
    {
        _hasher = hasher;
        _contextFactory = contextFactory;
    }

    public async ValueTask<BaseResponse<UserResponse>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (!command.Request.Password.IsStrongPassword())
        {
            return CustomStatusCodes.WeakPassword;
        }

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var isUsernameUsed = await context.Users.AnyAsync(e => e.Username == command.Request.Username, cancellationToken);
        if (isUsernameUsed)
        {
            return CustomStatusCodes.UsernameAlreadyUsing;
        }

        var passwordHash = _hasher.ComputeHash(command.Request.Password);
        var user = new User
        {
            Username = command.Request.Username,
            PasswordHash = passwordHash
        };
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return user.MapToResponse();
    }
}