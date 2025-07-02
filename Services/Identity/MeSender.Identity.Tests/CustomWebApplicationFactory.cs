using Dapper;
using MeSender.Identity.Data;
using MeSender.Identity.WebApi.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Xunit;

namespace MeSender.Identity.ComponentTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<IController>, IAsyncLifetime
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
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _connectionString,
                ["Jwt:SecretKey"] = "your-super-secret-key-with-at-least-32-characters",
                ["Jwt:Issuer"] = "MeSender",
                ["Jwt:Audience"] = "MeSenderUsers",
                ["Jwt:AccessTokenExpirationMinutes"] = "1",
                ["Jwt:RefreshTokenExpirationMinutes"] = "5",
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
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = ["VersionInfo"],
        });
    }

    private async Task InitializeDatabaseSchemaAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync("""
                                          CREATE TABLE IF NOT EXISTS "Users"
                                      (
                                          Id        UUID PRIMARY KEY,
                                          CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL
                                      );

                                      CREATE TABLE IF NOT EXISTS "UserAuth"
                                      (
                                          Id                      UUID         PRIMARY KEY,
                                          UserId                  UUID         NOT NULL,
                                          Email                   VARCHAR(255) NOT NULL,
                                          Password                TEXT         NOT NULL,
                                          Salt                    TEXT         NOT NULL,
                                          RefreshToken            TEXT         NULL,
                                          RefreshTokenExpiresAt   TIMESTAMP    WITH TIME ZONE NULL,
                                          CONSTRAINT FK_UserAuthentications_Users FOREIGN KEY (UserId) REFERENCES "Users" (Id),
                                          CONSTRAINT UQ_UserAuthentications_Email UNIQUE (Email)
                                      );
                                      """);
    }

    public new async Task DisposeAsync() => await _postgresContainer.DisposeAsync();

    public async Task ResetDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }
}
