using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Constants;
using Application.DTO.Responses.Auth;
using Application.Options;
using Application.Services.JWT.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.JWT;

public class JwtService : IJwtService
{
    private readonly IOptions<JwtOptions> _options;

    public JwtService(IOptions<JwtOptions> options)
    {
        _options = options;
    }

    public string GenerateAccessToken(UserClaimsCollection claimsCollection)
    {
        var options = _options.Value;
        var notBefore = DateTime.UtcNow;
        var expires = notBefore.Add(options.AccessTokenLifetime);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, claimsCollection.Username),
            new(CustomClaimTypes.UserId, claimsCollection.Id)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecurityKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(options.Issuer, options.Audience, claims, notBefore, expires, credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public AuthTokenResponse GenerateAuthToken(UserClaimsCollection claimsCollection)
    {
        return new AuthTokenResponse
        {
            AccessToken = GenerateAccessToken(claimsCollection)
        };
    }
}