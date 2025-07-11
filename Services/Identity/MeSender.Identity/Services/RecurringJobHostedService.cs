using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MeSender.Identity.Services;

public sealed class RecurringJobHostedService(
    IRecurringJobManager recurringJobManager,
    IServiceScopeFactory scopeFactory) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        recurringJobManager.AddOrUpdate(
            "delete-expired-refresh-tokens",
            () => userService.DeleteExpiredRefreshTokensAsync(),
            Cron.Daily);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
