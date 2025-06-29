using System.Data.Common;

namespace MeSender.Identity.Data;

public interface IDbConnectionFactory
{
    public DbConnection CreateConnection();
}
