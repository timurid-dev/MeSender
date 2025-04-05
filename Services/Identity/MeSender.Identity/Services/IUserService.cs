namespace MeSender.Identity.Services;

public interface IUserService
{
    Task<bool> AddUserAsync(string email, string password);

    Task<(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt)> LoginUserAsync(string email, string password);
}
