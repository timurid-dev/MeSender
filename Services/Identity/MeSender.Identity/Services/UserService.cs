using MeSender.Identity.Models;
using MeSender.Identity.Repositories;

namespace MeSender.Identity.Services;

internal sealed class UserService(IUserRepository userRepository, ITokenService tokenService, TimeProvider timeProvider, IPasswordService passwordService)
    : IUserService
{
    public async Task<bool> AddUserAsync(string email, string password)
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

    public async Task<TokenPair?> LoginUserAsync(string email, string password)
    {
        var authData = await userRepository.LoginUserAsync(email);

        if (authData == null)
        {
            return null;
        }

        var success = passwordService.VerifyPassword(password, authData.Password, authData.Salt);
        return success ? tokenService.GenerateTokens(authData.Id, email) : null;
    }
}
