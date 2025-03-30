using Dapper;
using MeSender.Identity.Data;
using MeSender.Identity.Models;

namespace MeSender.Identity.Repositories;

internal sealed class UserRepository(string connectionString)
{
    public async Task<bool> AddUserAsync(UserEntity user)
    {
        var dbContext = new IdentityDbContext(connectionString);
        using var connection = dbContext.CreateConnection();

        const string checkSql = """SELECT 1 FROM "Users" WHERE Email = @Email""";
        var exists = await connection.ExecuteScalarAsync<bool>(checkSql, new
        {
            user.Email,
        });

        if (exists)
        {
            return false;
        }

        const string insertSql = """INSERT INTO "Users" (Id, Email, Password) VALUES (@Id, @Email, @Password)""";
        return await connection.ExecuteAsync(insertSql, user) > 0;
    }
}
