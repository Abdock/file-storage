using Application.DTO.Responses.Auth;
using Application.Services.JWT.Models;

namespace Application.Services.JWT;

public interface IJwtService
{
    string GenerateAccessToken(UserClaimsCollection claimsCollection);
    AuthTokenResponse GenerateAuthToken(UserClaimsCollection claimsCollection);
}