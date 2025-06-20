using System.Reflection;
using Dapper;
using MeSender.Identity.Data;
using Microsoft.Extensions.Hosting;

namespace MeSender.Identity.Services;

public sealed class MigrationService(string connectionString) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase));

        foreach (var resourceName in resourceNames)
        {
            await using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
            var sqlScript = await reader.ReadToEndAsync(cancellationToken);

            using var connection = new DbConnectionFactory(connectionString).CreateConnection();
            await connection.ExecuteAsync(sqlScript);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
