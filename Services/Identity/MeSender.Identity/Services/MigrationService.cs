using System.Reflection;
using Dapper;
using MeSender.Identity.Data;
using Microsoft.Extensions.Hosting;

namespace MeSender.Identity.Services;

public sealed class MigrationService(IDbConnectionFactory dbConnectionFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            .Order();

        foreach (var resourceName in resourceNames)
        {
            await using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
            var sqlScript = await reader.ReadToEndAsync(cancellationToken);

            await using var connection = dbConnectionFactory.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            await connection.ExecuteAsync(sqlScript);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
