using Application.DTO.Requests.General;

namespace Application.DTO.Requests.FileAttachments;

public sealed record GetUserFileAttachmentsRequest : AuthorizedApiRequest, IPaginationRequest
{
    public required int Take { get; init; }
    public required int Skip { get; init; }
}