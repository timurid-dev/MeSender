using MeSender.Identity.Models;

namespace MeSender.Identity.Services;

public interface IUserService
{
    Task<bool> AddUserAsync(string email, string password);

    Task<TokenPair?> LoginUserAsync(string email, string password);
}
