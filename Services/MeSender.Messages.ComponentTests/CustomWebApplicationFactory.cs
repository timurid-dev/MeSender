using MeSender.Messages.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace MeSender.Messages.ComponentTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<WebApi.Controllers.MessagesController>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ChatDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ChatDbContext>(options => options.UseInMemoryDatabase("TestInmemoryDB"));
        });
    }

    internal async Task ClearDatabaseAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        dbContext.Messages.RemoveRange(dbContext.Messages);
        await dbContext.SaveChangesAsync();
    }
}
