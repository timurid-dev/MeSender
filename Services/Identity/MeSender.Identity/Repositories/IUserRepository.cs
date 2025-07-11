using CSharpFunctionalExtensions;
using MeSender.Identity.Models;

namespace MeSender.Identity.Repositories;

public interface IUserRepository
{
    public Task<Result> AddUserAsync(UserEntity user);

    public Task<AuthData?> LoginUserAsync(string email);

    public Task AddRefreshTokenAsync(Guid userId, string refreshToken, DateTimeOffset expiresAt);

    public Task<RefreshTokenData?> FindRefreshTokenAsync(string refreshToken);

    public Task<int> DeleteExpiredRefreshTokensAsync(DateTimeOffset dateTimeOffset);
}
