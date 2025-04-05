using System.Data;
using Dapper;
using MeSender.Identity.Data;
using MeSender.Identity.Models;
using MeSender.Identity.Services;

namespace MeSender.Identity.Repositories;

internal sealed class UserRepository(string connectionString)
{
    public async Task<bool> AddUserAsync(UserEntity user)
    {
        var connection = CreateDbConnection();

        const string checkSql = """SELECT 1 FROM "UserAuth" WHERE Email = @Email""";
        var exists = await connection.ExecuteScalarAsync<bool>(checkSql, new
        {
            user.Email,
        });

        if (exists)
        {
            return false;
        }

        var (passwordHash, salt) = PasswordService.HashPassword(user.Password);

        const string insertUserSql = """INSERT INTO "Users" (Id, CreatedAt) VALUES (@Id, @CreatedAt)""";
        await connection.ExecuteAsync(insertUserSql, user);

        const string insertAuthSql = """INSERT INTO "UserAuth" (Id, UserId, Email, Password, Salt) VALUES (@AuthId, @Id, @Email, @Password, @Salt)""";
        var authData = new
        {
            AuthId = Guid.NewGuid(),
            user.Id,
            user.Email,
            Password = passwordHash,
            Salt = salt,
        };

        return await connection.ExecuteAsync(insertAuthSql, authData) > 0;
    }

    public async Task<Guid?> LoginUserAsync(string email, string password)
    {
        var connection = CreateDbConnection();

        const string getSql = """SELECT Id, Password, Salt FROM "UserAuth" WHERE Email = @Email""";
        var authData = await connection.QuerySingleOrDefaultAsync<(Guid Id, string Password, string Salt)>(getSql, new
        {
            email,
        });

        if (authData.Password.Length != 0 &&
            PasswordService.VerifyPassword(password, authData.Password, authData.Salt))
        {
            return authData.Id;
        }

        return null;
    }

    private IDbConnection CreateDbConnection()
    {
        var dbContext = new IdentityDbContext(connectionString);
        return dbContext.CreateConnection();
    }
}
