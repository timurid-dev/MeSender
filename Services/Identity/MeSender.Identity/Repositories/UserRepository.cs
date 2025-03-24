using Dapper;
using MeSender.Identity.Data;
using MeSender.Identity.Models;
using Npgsql;

namespace MeSender.Identity.Repositories;

internal sealed class UserRepository(string connectionString)
{
    public void AddUser(UserEntity user)
    {
        var dbContext = new IdentityDbContext(connectionString);
        var connection = dbContext.CreateConnection();
        const string sql = "INSERT INTO Users (Id, Email, Password) VALUES (@Id, @Email, @Password)";
        connection.Execute(sql, user);
    }
}
