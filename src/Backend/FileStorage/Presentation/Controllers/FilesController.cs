using Application.CQRS.Queries.FileAttachments;
using Application.DTO.Requests.FileAttachments;
using Application.DTO.Responses.FileAttachments;
using Application.DTO.Responses.General;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes.Authorization;
using Presentation.DTO.Requests.General;
using Presentation.DTO.Responses.General;
using Presentation.Extensions;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class FilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [ApiKeyAuthorize]
    [HttpGet]
    [Route("{fileName:alpha}")]
    [ProducesResponseType<FileResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFileContent([FromRoute] string fileName, CancellationToken cancellationToken)
    {
        var request = new GetFileAttachmentRequest
        {
            FileName = fileName,
            Authorization = HttpContext.GetAuthorizedRequestOrDefault()!
        };
        var query = new GetFileAttachmentQuery
        {
            Request = request
        };
        var response = await _mediator.Send(query, cancellationToken);
        if (!response.IsSuccess)
        {
            return response.StatusCode.MapToActionResult();
        }

        await using var content = response.Response!;
        return File(content.Content, content.MimeType);
    }

    [ApiKeyAuthorize]
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<FileAttachmentResponse>>>> GetFiles([FromQuery] PaginationRequest pagination, CancellationToken cancellationToken)
    {
        var request = new GetUserFileAttachmentsRequest
        {
            Take = pagination.Take,
            Skip = pagination.Skip,
            Authorization = HttpContext.GetAuthorizedRequestOrDefault()!
        };
        var query = new GetUserFileAttachmentsQuery
        {
            Request = request
        };
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}