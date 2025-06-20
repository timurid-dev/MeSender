using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MeSender.Identity.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MeSender.Identity.Services;

internal sealed class TokenService(IOptions<JwtOptions> jwtOptions, TimeProvider timeProvider)
{
    public TokenPair GenerateTokens(Guid userId, string email)
    {
        var accessToken = GenerateJwtToken(userId, email);
        var refreshToken = GenerateRefreshToken();

        return new TokenPair
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.Expires,
            RefreshToken = refreshToken,
        };
    }

    private (string Token, DateTimeOffset Expires) GenerateJwtToken(Guid userId, string email)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            expires: timeProvider.GetUtcNow().LocalDateTime.AddMinutes(jwtOptions.Value.AccessTokenExpirationMinutes),
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), new DateTimeOffset(token.ValidTo));
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
