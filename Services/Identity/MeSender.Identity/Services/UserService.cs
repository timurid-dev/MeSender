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

    public async Task<Result<TokenPair>> LoginUserAsync(string email, string password)
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

        var tokenPair = tokenService.GenerateTokens(authData.UserId, email);
        await userRepository.UpdateRefreshTokenAsync(authData.UserId, tokenPair.RefreshToken, tokenPair.RefreshTokenExpiresAt);
        return Result.Success(tokenPair);
    }
}
