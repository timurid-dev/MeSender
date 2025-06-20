using MeSender.Identity.Data;
using MeSender.Identity.Models;
using MeSender.Identity.Repositories;
using MeSender.Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeSender.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUsers(this IServiceCollection services, string connectionString)
    {
        var dbConnection = new DbConnectionFactory(connectionString);
        var connection = dbConnection.CreateConnection();

        services.AddScoped<UserRepository>(_ => new UserRepository(connection));
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<TokenService>();
        services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName);
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }
}
