using Dapper;
using Npgsql;

namespace MigrationWorker;

internal sealed class Worker(ILogger<Worker> logger, string connectionString) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting database migration...");

        var sqlFilePath = Path.Combine(AppContext.BaseDirectory, "CreateTable_Users.sql");
        var sqlScript = await File.ReadAllTextAsync(sqlFilePath, stoppingToken);

        await using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync(stoppingToken);
            await connection.ExecuteAsync(sqlScript);
        }

        logger.LogInformation("Database migration completed.");
    }
}
