using System.Diagnostics;
using CSharpFunctionalExtensions;
using Dapper;
using MeSender.Identity.Data;
using MeSender.Identity.Models;

namespace MeSender.Identity.Repositories;

internal sealed class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
{
    private static readonly ActivitySource ActivitySource = new($"{nameof(MeSender)}.{nameof(Identity)}.{nameof(UserRepository)}");

    public async Task<Result> AddUserAsync(UserEntity user)
    {
        using var activity = ActivitySource.StartActivity();
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
                return Result.Failure("User with that email already exists.");
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

            await connection.ExecuteAsync(insertAuthSql, authData, transaction);
            await transaction.CommitAsync();

            return Result.Success();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<AuthData?> LoginUserAsync(string email)
    {
        using var activity = ActivitySource.StartActivity();
        await using var connection = await connectionFactory.OpenConnectionAsync();
        const string getSql = """SELECT UserId, Email, Password, Salt FROM "UserAuth" WHERE Email = @Email""";
        return await connection.QuerySingleOrDefaultAsync<AuthData>(getSql, new
        {
            email,
        });
    }

    public async Task AddRefreshTokenAsync(Guid userId, string refreshToken, DateTimeOffset expiresAt)
    {
        using var activity = ActivitySource.StartActivity();
        await using var connection = await connectionFactory.OpenConnectionAsync();
        const string sql = """
            INSERT INTO "UserRefreshTokens" (Id, UserId, RefreshToken, ExpiresAt)
            VALUES (@Id, @UserId, @RefreshToken, @ExpiresAt)
        """;
        await connection.ExecuteAsync(sql, new
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
        });
    }

    public async Task<RefreshTokenData?> FindRefreshTokenAsync(string refreshToken)
    {
        using var activity = ActivitySource.StartActivity();
        await using var connection = await connectionFactory.OpenConnectionAsync();
        const string sql = """
            SELECT Id, UserId, RefreshToken, ExpiresAt
            FROM "UserRefreshTokens"
            WHERE RefreshToken = @RefreshToken
        """;
        return await connection.QuerySingleOrDefaultAsync<RefreshTokenData>(sql, new
        {
            RefreshToken = refreshToken,
        });
    }

    public async Task<int> DeleteExpiredRefreshTokensAsync(DateTimeOffset dateTimeOffset)
    {
        using var activity = ActivitySource.StartActivity();
        await using var connection = await connectionFactory.OpenConnectionAsync();
        const string sql = """
                           DELETE FROM "UserRefreshTokens" WHERE ExpiresAt < @UtcNow
                           """;
        return await connection.ExecuteAsync(sql, new
        {
            UtcNow = dateTimeOffset,
        });
    }
}
