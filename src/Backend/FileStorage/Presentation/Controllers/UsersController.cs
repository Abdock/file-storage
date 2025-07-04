using Application.CQRS.Queries.Users;
using Application.DTO.Requests.General;
using Application.DTO.Responses.General;
using Application.DTO.Responses.Users;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTO.Responses.General;
using Presentation.Extensions;

namespace Presentation.Controllers;

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
}