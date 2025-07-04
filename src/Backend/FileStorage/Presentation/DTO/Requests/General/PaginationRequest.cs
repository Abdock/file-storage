using System.ComponentModel.DataAnnotations;
using Application.DTO.Requests.General;

namespace Presentation.DTO.Requests.General;

public record PaginationRequest : IPaginationRequest
{
    [Range(1, 1000)]
    public int Take { get; init; } = 50;
    public int Skip { get; init; } = 0;
}