using Application.DTO.Requests.General;
using Application.DTO.Responses.General;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResponse<TData>> ApplyPaginationAsync<TData>(this IQueryable<TData> query, IPaginationRequest pagination, CancellationToken cancellationToken = default)
    {
        return new PagedResponse<TData>
        {
            Total = await query.CountAsync(cancellationToken),
            Data = await query.Skip(pagination.Skip).Take(pagination.Take).ToArrayAsync(cancellationToken)
        };
    }
}