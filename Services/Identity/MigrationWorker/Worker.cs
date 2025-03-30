using Dapper;
using Npgsql;

namespace MigrationWorker;

internal sealed class Worker(ILogger<Worker> logger, string connectionString) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Начинается миграция базы данных...");

        var sqlDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Queries");
        var sqlFiles = Directory.GetFiles(sqlDirectoryPath, "*.sql");

        await using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync(stoppingToken);

            foreach (var sqlFilePath in sqlFiles)
            {
                var sqlScript = await File.ReadAllTextAsync(sqlFilePath, stoppingToken);
                await connection.ExecuteAsync(sqlScript);
                logger.LogInformation($"Выполнен SQL скрипт: {Path.GetFileName(sqlFilePath)}");
            }
        }

        logger.LogInformation("Миграция базы данных завершена.");
    }
}
