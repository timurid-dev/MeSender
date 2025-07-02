using CSharpFunctionalExtensions;
using MeSender.Identity.Models;

namespace MeSender.Identity.Repositories;

public interface IUserRepository
{
    public Task<Result> AddUserAsync(UserEntity user);

    public Task<AuthData?> LoginUserAsync(string email);

    public Task<RefreshTokenData?> FindByRefreshTokenAsync(string refreshToken);

    public Task UpdateRefreshTokenAsync(Guid userId, string newRefreshToken, DateTimeOffset expiresAt);
}
