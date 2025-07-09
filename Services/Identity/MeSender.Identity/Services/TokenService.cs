using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CSharpFunctionalExtensions;
using MeSender.Identity.Models;
using MeSender.Identity.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MeSender.Identity.Services;

internal sealed class TokenService(IOptions<JwtOptions> jwtOptions, TimeProvider timeProvider) : ITokenService
{
    public TokenPair GenerateTokens(Guid userId)
    {
        var accessToken = GenerateJwtToken(userId);
        var refreshToken = GenerateRefreshToken();

        return new TokenPair
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.Expires,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = timeProvider.GetUtcNow().Add(jwtOptions.Value.RefreshTokenExpirationSpan),
        };
    }

    private (string Token, DateTimeOffset Expires) GenerateJwtToken(Guid userId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            expires: timeProvider.GetUtcNow().ToLocalTime().Add(jwtOptions.Value.AccessTokenExpirationSpan).DateTime,
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
