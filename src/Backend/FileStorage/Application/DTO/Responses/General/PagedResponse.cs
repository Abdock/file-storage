namespace Application.DTO.Responses.General;

public sealed record PagedResponse<TResponse>
{
    public required int Total { get; init; }
    public required IReadOnlyCollection<TResponse> Data { get; init; }
}