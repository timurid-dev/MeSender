using System.Data.Common;
using Npgsql;

namespace MeSender.Identity.Data;

public sealed class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
