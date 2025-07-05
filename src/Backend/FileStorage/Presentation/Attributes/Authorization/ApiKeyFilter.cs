using Application.DTO.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Presentation.Constants;
using Presentation.Extensions;

namespace Presentation.Attributes.Authorization;

public sealed class ApiKeyFilter : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(HttpHeaderConstants.ApiKey, out var value))
        {
            context.Result = CustomStatusCodes.ApiKeyWasNotFound.AsErrorActionResult();
            return;
        }

        var cancellationToken = context.HttpContext.RequestAborted;
        var dbContextFactory = context.HttpContext.RequestServices.GetRequiredService<IDbContextFactory<StorageContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var token = value.ToString();
        var key = await dbContext.ApiKeys.FirstOrDefaultAsync(e => e.Token == token, cancellationToken);
        if (key is null)
        {
            context.Result = CustomStatusCodes.ApiKeyWasNotFound.AsErrorActionResult();
            return;
        }

        context.HttpContext.Features.Set(key);
    }
}