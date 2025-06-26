using Application.DTO.Enums;
using Application.DTO.Requests.Users;
using Application.DTO.Responses.Auth;
using Application.DTO.Responses.General;
using Application.Extensions;
using Application.Services.Hashing;
using Application.Services.JWT;
using Application.Services.JWT.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Entities;

namespace Application.CQRS.Commands.Auth;

public class CreateUserCommand : ICommand<BaseResponse<AuthTokenResponse>>
{
    public required CreateUserRequest Request { get; init; }
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, BaseResponse<AuthTokenResponse>>
{
    private readonly IHasher _hasher;
    private readonly IDbContextFactory<StorageContext> _contextFactory;
    private readonly IJwtService _jwtService;

    public CreateUserCommandHandler(IHasher hasher, IDbContextFactory<StorageContext> contextFactory, IJwtService jwtService)
    {
        _hasher = hasher;
        _contextFactory = contextFactory;
        _jwtService = jwtService;
    }

    public async ValueTask<BaseResponse<AuthTokenResponse>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
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
        var claims = new UserClaimsCollection
        {
            Id = user.Id.ToString(),
            Username = user.Username
        };
        return _jwtService.GenerateAuthToken(claims);
    }
}