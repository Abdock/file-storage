﻿namespace Application.DTO.Responses.ApiKeys;

public sealed record ApiKeyResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Token { get; init; }
}