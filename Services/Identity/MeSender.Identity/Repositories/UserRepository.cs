using System.Data;
using Dapper;
using MeSender.Identity.Models;

namespace MeSender.Identity.Repositories;

internal sealed class UserRepository(IDbConnection connection)
{
    public async Task<bool> AddUserAsync(UserEntity user, string salt)
    {
        const string checkSql = """SELECT 1 FROM "UserAuth" WHERE Email = @Email""";
        var exists = await connection.ExecuteScalarAsync<bool>(checkSql, new
        {
            user.Email,
        });

        if (exists)
        {
            return false;
        }

        const string insertUserSql = """INSERT INTO "Users" (Id, CreatedAt) VALUES (@Id, @CreatedAt)""";
        await connection.ExecuteAsync(insertUserSql, user);

        const string insertAuthSql = """INSERT INTO "UserAuth" (Id, UserId, Email, Password, Salt) VALUES (@AuthId, @Id, @Email, @Password, @Salt)""";
        var authData = new
        {
            AuthId = Guid.NewGuid(),
            user.Id,
            user.Email,
            user.Password,
            Salt = salt,
        };

        return await connection.ExecuteAsync(insertAuthSql, authData) > 0;
    }

    public async Task<AuthData?> LoginUserAsync(string email)
    {
        const string getSql = """SELECT Id, Password, Salt FROM "UserAuth" WHERE Email = @Email""";
        return await connection.QuerySingleOrDefaultAsync<AuthData>(getSql, new
        {
            email,
        });
    }
}
