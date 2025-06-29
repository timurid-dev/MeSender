using MeSender.Identity.Models;

namespace MeSender.Identity.Services;

public interface IUserService
{
    public Task<bool> AddUserAsync(string email, string password);

    public Task<TokenPair?> LoginUserAsync(string email, string password);
}
