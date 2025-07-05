using Application.CQRS.Commands.ApiKeys;
using Application.DTO.Requests.ApiKeys;
using Application.DTO.Responses.ApiKeys;
using Application.DTO.Responses.General;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTO.Inputs.ApiKeys;
using Presentation.DTO.Responses.General;
using Presentation.Extensions;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiKeysController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApiKeysController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType<BaseResponse<ApiKeyResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateApiKey([FromBody] CreateApiKeyInput input, CancellationToken cancellationToken)
    {
        var request = new CreateApiKeyRequest
        {
            Name = input.Name,
            Permissions = input.Permissions,
            Authorization = User.GetAuthorizedUser()
        };
        var command = new CreateApiKeyCommand
        {
            Request = request
        };
        var response = await _mediator.Send(command, cancellationToken);
        return response.IsSuccess ? Ok(response) : response.AsActionResult();
    }

    [Authorize]
    [HttpPost]
    [Route("revoke/{id:guid}")]
    [ProducesResponseType<BaseResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RevokeApiKey([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new RevokeApiKeyRequest
        {
            Authorization = User.GetAuthorizedUser(),
            Id = id
        };
        var command = new RevokeApiKeyCommand
        {
            Request = request
        };
        var response = await _mediator.Send(command, cancellationToken);
        return response.IsSuccess ? Ok(response) : response.AsActionResult();
    }
}