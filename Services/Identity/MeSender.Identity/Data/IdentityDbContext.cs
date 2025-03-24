using System.Data;
using Npgsql;

namespace MeSender.Identity.Data;

internal sealed class IdentityDbContext(string connectionString)
{
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}
