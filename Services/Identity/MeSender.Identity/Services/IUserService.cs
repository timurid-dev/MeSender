using CSharpFunctionalExtensions;
using MeSender.Identity.Models;

namespace MeSender.Identity.Services;

public interface IUserService
{
    public Task<Result> AddUserAsync(string email, string password);

    public Task<Result<TokenPair>> LoginUserAsync(string email, string password, string provider);

    public Task<Result<TokenPair>> RefreshUserTokenAsync(string refreshToken);

    public Task<int> DeleteExpiredRefreshTokensAsync();
}
