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

internal sealed class TokenService(IOptions<JwtOptions> jwtOptions, IUserRepository userRepository, TimeProvider timeProvider) : ITokenService
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
            RefreshTokenExpiresAt = timeProvider.GetUtcNow().AddMinutes(jwtOptions.Value.AccessTokenExpirationMinutes),
        };
    }

    public async Task<Result<TokenPair>> RefreshTokensAsync(string email, string refreshToken)
    {
        var refreshTokenData = await userRepository.FindByRefreshTokenAsync(refreshToken);
        var utcNow = timeProvider.GetUtcNow();
        if (refreshTokenData == null || refreshTokenData.RefreshToken != refreshToken || refreshTokenData.RefreshTokenExpiresAt < utcNow)
        {
            return Result.Failure<TokenPair>("The refresh token is invalid or expired");
        }

        var tokenPair = GenerateTokens(refreshTokenData.UserId, email);
        await userRepository.UpdateRefreshTokenAsync(refreshTokenData.UserId, tokenPair.RefreshToken, utcNow.AddMinutes(jwtOptions.Value.RefreshTokenExpirationMinutes));
        return Result.Success(tokenPair);
    }

    private (string Token, DateTimeOffset Expires) GenerateJwtToken(Guid userId, string email)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString() + Guid.NewGuid()),
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
