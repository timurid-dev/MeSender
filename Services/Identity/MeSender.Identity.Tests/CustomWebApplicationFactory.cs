using System.Data.Common;
using MeSender.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Testcontainers.PostgreSql;
using Xunit;

namespace MeSender.Identity.Tests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15")
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .WithCleanUp(true)
        .Build();

    private string _connectionString = null!;
    private Respawner _respawner = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _connectionString,
                ["Jwt:SecretKey"] = "your-super-secret-key-with-at-least-32-characters",
                ["Jwt:Issuer"] = "MeSender",
                ["Jwt:Audience"] = "MeSenderUsers",
                ["Jwt:AccessTokenExpirationSpan"] = "00:02:00",
                ["Jwt:RefreshTokenExpirationSpan"] = "00:05:00",
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDbConnectionFactory));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(_connectionString));
        });
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        _connectionString = _postgresContainer.GetConnectionString();
        await InitializeDatabaseSchemaAsync();
        var connection = await CreateDbConnectionAsync(_connectionString);

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = ["VersionInfo"],
        });
    }

    private async Task InitializeDatabaseSchemaAsync()
    {
        var migrationService = new Services.MigrationService(GetDbConnectionFactory(_connectionString));
        await migrationService.StartAsync(CancellationToken.None);
    }

    public new async Task DisposeAsync() => await _postgresContainer.DisposeAsync();

    public async Task ResetDatabaseAsync()
    {
        var connection = await CreateDbConnectionAsync(_connectionString);
        await _respawner.ResetAsync(connection);
    }

    private static async Task<DbConnection> CreateDbConnectionAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        return await GetDbConnectionFactory(connectionString).OpenConnectionAsync(cancellationToken);
    }

    private static IDbConnectionFactory GetDbConnectionFactory(string connectionString)
    {
        return new DbConnectionFactory(connectionString);
    }
}
