using Application.CQRS.Commands.Auth;
using Application.CQRS.Queries.ApiKeys;
using Application.CQRS.Queries.Users;
using Application.DTO.Requests.Auth;
using Application.DTO.Requests.General;
using Application.DTO.Requests.Users;
using Application.DTO.Responses.ApiKeys;
using Application.DTO.Responses.Auth;
using Application.DTO.Responses.General;
using Application.DTO.Responses.Users;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTO.Inputs.Users;
using Presentation.DTO.Responses.General;
using Presentation.Extensions;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet]
    [Route("me")]
    [ProducesResponseType<BaseResponse<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponse<UserResponse>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUser(CancellationToken cancellationToken)
    {
        var request = new AuthorizedUserRequest
        {
            UserId = User.GetUserId()
        };
        var query = new GetAuthorizedUserQuery
        {
            Request = request
        };
        var response = await _mediator.Send(query, cancellationToken);
        return response.IsSuccess ? Ok(response) : response.AsActionResult();
    }

    [Authorize]
    [HttpGet]
    [Route("me/api-keys")]
    [ProducesResponseType<BaseResponse<IReadOnlyCollection<ApiKeyResponse>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponse<IReadOnlyCollection<ApiKeyResponse>>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserApiKeys(CancellationToken cancellationToken)
    {
        var request = new AuthorizedUserRequest
        {
            UserId = User.GetUserId()
        };
        var query = new GetUserApiKeysQuery
        {
            Request = request
        };
        var response = await _mediator.Send(query, cancellationToken);
        return response.IsSuccess ? Ok(response) : response.AsActionResult();
    }

    [HttpPost]
    [Route("login")]
    [ProducesResponseType<BaseResponse<AuthTokenResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponse<AuthTokenResponse>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginInput input, CancellationToken cancellationToken)
    {
        var request = new LoginRequest
        {
            Username = input.Username,
            Password = input.Password
        };
        var command = new LoginCommand
        {
            Request = request
        };
        var response = await _mediator.Send(command, cancellationToken);
        return response.IsSuccess ? Ok(response) : response.AsActionResult();
    }

    [HttpPost]
    [Route("register")]
    [ProducesResponseType<BaseResponse<AuthTokenResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponse<AuthTokenResponse>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterInput input, CancellationToken cancellationToken)
    {
        var request = new CreateUserRequest
        {
            Username = input.Username,
            Password = input.Password
        };
        var command = new CreateUserCommand
        {
            Request = request
        };
        var response = await _mediator.Send(command, cancellationToken);
        return response.IsSuccess ? Ok(response) : response.AsActionResult();
    }
}