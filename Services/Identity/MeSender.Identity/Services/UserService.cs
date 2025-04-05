using MeSender.Identity.Models;
using MeSender.Identity.Repositories;

namespace MeSender.Identity.Services;

internal sealed class UserService(UserRepository userRepository, TokenService tokenService, TimeProvider timeProvider)
    : IUserService
{
    public async Task<bool> AddUserAsync(string email, string password)
    {
        var userEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = email,
            Password = password,
            CreatedAt = timeProvider.GetUtcNow(),
        };
        return await userRepository.AddUserAsync(userEntity);
    }

    public async Task<(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt)> LoginUserAsync(string email, string password)
    {
        var userId = await userRepository.LoginUserAsync(email, password);
        if (userId == null)
        {
            return (string.Empty, string.Empty, DateTimeOffset.MinValue);
        }

        var tokens = tokenService.GenerateTokens(userId.Value, email);
        return (tokens.AccessToken, tokens.RefreshToken, tokens.ExpiresAt);
    }
}
