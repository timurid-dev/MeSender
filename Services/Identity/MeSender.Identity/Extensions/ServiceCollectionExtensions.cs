using Hangfire;
using Hangfire.Redis.StackExchange;
using MeSender.Identity.Models;
using MeSender.Identity.Repositories;
using MeSender.Identity.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeSender.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUsers(this IServiceCollection services, Action<JwtOptions> configureJwtOptions)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ITokenService, TokenService>();
        services.Configure(configureJwtOptions);
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }

    public static IServiceCollection AddTokenCleanupJob(this IServiceCollection services, string connectionString)
    {
        services.AddHangfire(config =>
        {
            config.UseSimpleAssemblyNameTypeSerializer();
            config.UseRecommendedSerializerSettings();
            config.UseRedisStorage(connectionString);
        });
        services.AddHangfireServer();
        return services;
    }
}
