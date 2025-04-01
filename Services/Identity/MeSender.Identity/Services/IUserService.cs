namespace MeSender.Identity.Services;

public interface IUserService
{
    Task<bool> AddUserAsync(string email, string password);

    Task<bool> LoginUserAsync(string email, string password);
}
