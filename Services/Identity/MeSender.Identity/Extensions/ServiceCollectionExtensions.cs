using MeSender.Identity.Repositories;
using MeSender.Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeSender.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUsers(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<UserRepository>(_ => new UserRepository(connectionString));
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
