using System.Data.Common;
using Npgsql;

namespace MeSender.Identity.Data;

public sealed class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public DbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}
