using Dapper;
using MeSender.Identity.Data;
using MeSender.Identity.Models;
using MeSender.Identity.Services;

namespace MeSender.Identity.Repositories;

internal sealed class UserRepository(string connectionString)
{
    public async Task<bool> AddUserAsync(UserEntity user)
    {
        var dbContext = new IdentityDbContext(connectionString);
        using var connection = dbContext.CreateConnection();

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
}
