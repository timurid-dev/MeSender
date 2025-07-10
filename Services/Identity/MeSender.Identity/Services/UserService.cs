using CSharpFunctionalExtensions;
using MeSender.Identity.Models;
using MeSender.Identity.Repositories;

namespace MeSender.Identity.Services;

internal sealed class UserService(IUserRepository userRepository, ITokenService tokenService, TimeProvider timeProvider, IPasswordService passwordService)
    : IUserService
{
    public async Task<Result> AddUserAsync(string email, string password)
    {
        var passwordData = passwordService.HashPassword(password);
        var userEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = email,
            Password = passwordData.PasswordHash,
            Salt = passwordData.Salt,
            CreatedAt = timeProvider.GetUtcNow(),
        };
        return await userRepository.AddUserAsync(userEntity);
    }

    public async Task<Result<TokenPair>> LoginUserAsync(string email, string password, string provider)
    {
        var authData = await userRepository.LoginUserAsync(email);
        if (authData == null)
        {
            return Result.Failure<TokenPair>("Invalid email");
        }

        var success = passwordService.VerifyPassword(password, authData.Password, authData.Salt);
        if (!success)
        {
            return Result.Failure<TokenPair>("Invalid password");
        }

        var tokenPair = await GetUpdatedTokenAsync(authData.UserId, provider);
        return Result.Success(tokenPair);
    }

    public async Task<Result<TokenPair>> RefreshUserTokenAsync(string refreshToken)
    {
        var refreshTokenData = await userRepository.FindRefreshTokenAsync(refreshToken);
        var utcNow = timeProvider.GetUtcNow();

        if (refreshTokenData == null || refreshTokenData.RefreshToken != refreshToken || refreshTokenData.ExpiresAt < utcNow)
        {
            return Result.Failure<TokenPair>("The refresh token is invalid or expired");
        }

        var tokenPair = await GetUpdatedTokenAsync(refreshTokenData.UserId, refreshTokenData.Provider);
        return Result.Success(tokenPair);
    }

    public async Task<int> DeleteExpiredRefreshTokensAsync()
    {
        var utcNow = timeProvider.GetUtcNow();
        return await userRepository.DeleteExpiredRefreshTokensAsync(utcNow);
    }

    private async Task<TokenPair> GetUpdatedTokenAsync(Guid userId, string provider)
    {
        var tokenPair = tokenService.GenerateTokens(userId);
        await userRepository.AddRefreshTokenAsync(userId, tokenPair.RefreshToken, tokenPair.RefreshTokenExpiresAt, provider);
        return tokenPair;
    }
}
