using System.Linq.Expressions;
using Application.DTO.Responses.ApiKeys;
using Persistence.Entities;

namespace Application.DTO.Mapping;

public static class ApiKeyMapping
{
    public static readonly Expression<Func<ApiKey, ApiKeyResponse>> MapToResponseQuery = apiKey => new ApiKeyResponse
    {
        Id = apiKey.Id,
        Name = apiKey.Name,
        Token = apiKey.Token,
    };

    public static ApiKeyResponse MapToResponse(this ApiKey apiKey) => new()
    {
        Id = apiKey.Id,
        Name = apiKey.Name,
        Token = apiKey.Token,
    };
}