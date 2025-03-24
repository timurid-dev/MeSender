using MeSender.Messages.Data;
using MeSender.Messages.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MeSender.Messages.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessages(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ChatDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IMessageService, MessageService>();
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}
