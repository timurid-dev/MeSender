using System.Data.Common;

namespace MeSender.Identity.Data;

public interface IDbConnectionFactory
{
    public Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);
}
