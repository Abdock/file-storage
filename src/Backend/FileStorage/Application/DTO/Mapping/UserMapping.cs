using System.Linq.Expressions;
using Application.DTO.Responses.Users;
using Persistence.Entities;

namespace Application.DTO.Mapping;

public static class UserMapping
{
    public static readonly Expression<Func<User, UserResponse>> MapToResponseQuery = user => new UserResponse
    {
        Id = user.Id,
        Username = user.Username
    };

    public static UserResponse MapToResponse(this User user) => new()
    {
        Id = user.Id,
        Username = user.Username
    };
}