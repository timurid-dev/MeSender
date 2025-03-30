using MeSender.Identity.Models;

namespace MeSender.Identity.Services;

public interface IUserService
{
    Task<bool> AddUserAsync(string email, string password);
}
