using Application.DTO.Enums;
using Application.DTO.Requests.Auth;
using Application.DTO.Responses.Auth;
using Application.DTO.Responses.General;
using Application.Services.Hashing;
using Application.Services.JWT;
using Application.Services.JWT.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.CQRS.Commands.Auth;

public sealed class LoginCommand : ICommand<BaseResponse<AuthTokenResponse>>
{
    public required LoginRequest Request { get; init; }
}

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, BaseResponse<AuthTokenResponse>>
{
    private readonly IDbContextFactory<StorageContext> _contextFactory;
    private readonly IHasher _hasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IDbContextFactory<StorageContext> contextFactory, IHasher hasher, IJwtService jwtService)
    {
        _contextFactory = contextFactory;
        _hasher = hasher;
        _jwtService = jwtService;
    }

    public async ValueTask<BaseResponse<AuthTokenResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Username == command.Request.Username, cancellationToken);
        if (user is null || !_hasher.Verify(command.Request.Password, user.PasswordHash))
        {
            return CustomStatusCodes.InvalidUserCredentials;
        }

        var claims = new UserClaimsCollection
        {
            Id = user.Id.ToString(),
            Username = user.Username
        };
        return _jwtService.GenerateAuthToken(claims);
    }
}