using MeSender.Identity.Models;
using MeSender.Identity.Repositories;

namespace MeSender.Identity.Services;

internal sealed class UserService(UserRepository userRepository, TimeProvider timeProvider) : IUserService
{
    public async Task<bool> AddUserAsync(string email, string password)
    {
        var userEntity = new UserEntity
        {
            Email = email,
            Password = password,
            CreatedAt = timeProvider.GetUtcNow(),
        };
        return await userRepository.AddUserAsync(userEntity);
    }
}
