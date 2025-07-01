using Dapper;
using MeSender.Identity.Data;
using MeSender.Identity.Models;

namespace MeSender.Identity.Repositories;

internal sealed class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
{
    public async Task<bool> AddUserAsync(UserEntity user)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            const string checkSql = """SELECT 1 FROM "UserAuth" WHERE Email = @Email""";
            var exists = await connection.ExecuteScalarAsync<bool>(checkSql, new
            {
                user.Email,
            }, transaction);

            if (exists)
            {
                await transaction.RollbackAsync();
                return false;
            }

            const string insertUserSql = """INSERT INTO "Users" (Id, CreatedAt) VALUES (@Id, @CreatedAt)""";
            await connection.ExecuteAsync(insertUserSql, new
            {
                user.Id, user.CreatedAt,
            }, transaction);

            const string insertAuthSql = """INSERT INTO "UserAuth" (Id, UserId, Email, Password, Salt) VALUES (@AuthId, @Id, @Email, @Password, @Salt)""";
            var authData = new
            {
                AuthId = Guid.NewGuid(),
                user.Id,
                user.Email,
                user.Password,
                user.Salt,
            };

            var result = await connection.ExecuteAsync(insertAuthSql, authData, transaction);
            await transaction.CommitAsync();

            return result > 0;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<AuthData?> LoginUserAsync(string email)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();
        const string getSql = """SELECT UserId, Email, Password, Salt FROM "UserAuth" WHERE Email = @Email""";
        return await connection.QuerySingleOrDefaultAsync<AuthData>(getSql, new
        {
            email,
        });
    }

    public async Task<RefreshTokenData?> FindByRefreshTokenAsync(string refreshToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();
        const string sql = """SELECT UserId, Email, Password, Salt, RefreshToken, RefreshTokenExpiresAt FROM "UserAuth" WHERE RefreshToken = @refreshToken""";
        return await connection.QuerySingleOrDefaultAsync<RefreshTokenData>(sql, new
        {
            refreshToken,
        });
    }

    public async Task UpdateRefreshTokenAsync(Guid userId, string newRefreshToken, DateTimeOffset expiresAt)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();

        const string sql = """UPDATE "UserAuth" SET RefreshToken = @newRefreshToken, RefreshTokenExpiresAt = @expiresAt WHERE UserId = @userId""";
        await connection.ExecuteAsync(sql, new
        {
            userId, newRefreshToken, expiresAt,
        });
    }
}
