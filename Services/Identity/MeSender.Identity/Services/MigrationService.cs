using System.Diagnostics;
using System.Reflection;
using Dapper;
using MeSender.Identity.Data;
using Microsoft.Extensions.Hosting;

namespace MeSender.Identity.Services;

public sealed class MigrationService(IDbConnectionFactory dbConnectionFactory) : IHostedService
{
    private static readonly ActivitySource ActivitySource = new($"{nameof(MeSender)}.{nameof(Identity)}.{nameof(MigrationService)}");

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity($"{nameof(MigrationService)}.{nameof(StartAsync)}");
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            .Order();

        foreach (var resourceName in resourceNames)
        {
            using var migrationActivity = ActivitySource.StartActivity($"Migration: {resourceName}");
            await using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
            var sqlScript = await reader.ReadToEndAsync(cancellationToken);

            await using var connection = await dbConnectionFactory.OpenConnectionAsync(cancellationToken);
            await connection.ExecuteAsync(sqlScript);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
