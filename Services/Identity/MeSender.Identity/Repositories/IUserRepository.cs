using MeSender.Identity.Models;

namespace MeSender.Identity.Repositories;

public interface IUserRepository
{
    public Task<bool> AddUserAsync(UserEntity user);

    public Task<AuthData?> LoginUserAsync(string email);
}
