using Application.DTO.Requests.General;

namespace Application.DTO.Requests.ApiKeys;

public record GetUserApiKeysRequest : AuthorizedUserRequest, IPaginationRequest
{
    public required int Take { get; init; }
    public required int Skip { get; init; }
}