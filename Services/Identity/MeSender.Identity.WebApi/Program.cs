using Hangfire;
using MeSender.Identity.Data;
using MeSender.Identity.Extensions;
using MeSender.Identity.Models;
using MeSender.Identity.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration["Jwt:Secret"] = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "your-super-secret-key-with-minimum-32-characters";
builder.Configuration["Jwt:RefreshSecret"] = Environment.GetEnvironmentVariable("JWT_REFRESH_SECRET") ?? "your-super-secret-refresh-key-with-minimum-32-characters";
builder.Configuration["Jwt:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "MeSender";
builder.Configuration["Jwt:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "MeSender";
builder.Configuration["Jwt:AccessTokenExpirationSpan"] = Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRATION") ?? "00:15:00";
builder.Configuration["Jwt:RefreshTokenExpirationSpan"] = Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRATION") ?? "7.00:00:00";

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException();
builder.Services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString));
builder.Services.AddHostedService<MigrationService>();
builder.Services.AddUsers(options => builder.Configuration.GetSection(JwtOptions.SectionName).Bind(options));
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MeSender.Identity.WebApi",
        Version = "v1",
    });
});

if (!builder.Environment.IsEnvironment("Testing"))
{
    var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException();
    builder.Services.AddTokenCleanupJob(redisConnectionString);
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "MeSender.Identity.WebApi v1"));
}

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHangfireDashboard();
    RecurringJob.AddOrUpdate<IUserService>(
        "delete-expired-refresh-tokens",
        service => service.DeleteExpiredRefreshTokensAsync(),
        Cron.Daily);
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();

public abstract partial class Program;
