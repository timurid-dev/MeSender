using System.Data;
using Npgsql;

namespace MeSender.Identity.Data;

internal sealed class DbConnectionFactory(string connectionString)
{
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}
