using Application.CQRS.Commands.FileAttachments;
using Application.CQRS.Queries.FileAttachments;
using Application.DTO.Requests.FileAttachments;
using Application.DTO.Responses.FileAttachments;
using Application.DTO.Responses.General;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using Presentation.Attributes.Authorization;
using Presentation.DTO.Inputs.Files;
using Presentation.DTO.Inputs.General;
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
            return response.AsErrorActionResult();
        }

        await using var content = response.Response!;
        return File(content.Content, content.MimeType);
    }

    [ApiKeyAuthorize]
    [HttpGet]
    [ProducesResponseType<BaseResponse<PagedResponse<FileAttachmentResponse>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResponse<PagedResponse<FileAttachmentResponse>>>> GetFiles([FromQuery] PaginationInput pagination, CancellationToken cancellationToken)
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

    [ApiKeyAuthorize]
    [HttpPost]
    [ProducesResponseType<BaseResponse<FileAttachmentResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponse<FileAttachmentResponse>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<BaseResponse<FileAttachmentResponse>>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadFile([FromBody] UploadFileInput input, CancellationToken cancellationToken)
    {
        var mimeType = MimeTypeMap.GetMimeType(input.File.FileName);
        await using var stream = input.File.OpenReadStream();
        var request = new CreateFileAttachmentRequest
        {
            Stream = stream,
            MimeType = mimeType,
            ExpiresAt = input.ExpiresAt,
            Authorization = HttpContext.GetAuthorizedRequestOrDefault()!
        };
        var command = new CreateFileAttachmentCommand
        {
            Request = request
        };
        var response = await _mediator.Send(command, cancellationToken);
        return response.IsSuccess ? Ok(response) : response.AsActionResult();
    }

    [ApiKeyAuthorize]
    [HttpDelete]
    [Route("{id:guid}")]
    [ProducesResponseType<BaseResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<BaseResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<BaseResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteFile([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteFileAttachmentRequest
        {
            Id = id,
            Authorization = HttpContext.GetAuthorizedRequestOrDefault()!
        };
        var command = new DeleteFileAttachmentCommand
        {
            Request = request
        };
        var response = await _mediator.Send(command, cancellationToken);
        return response.IsSuccess ? Ok(response) : response.AsActionResult();
    }
}