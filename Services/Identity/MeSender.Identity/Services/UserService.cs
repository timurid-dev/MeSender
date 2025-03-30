using MeSender.Identity.Models;
using MeSender.Identity.Repositories;

namespace MeSender.Identity.Services;

internal sealed class UserService(UserRepository userRepository) : IUserService
{
    public async Task<bool> AddUserAsync(string email, string password)
    {
        var userEntity = new UserEntity
        {
            Email = email,
            Password = password,
        };
        return await userRepository.AddUserAsync(userEntity);
    }
}
