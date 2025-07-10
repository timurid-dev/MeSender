using System.Text;
using MeSender.Identity.Models;
using MeSender.Messages.Data;
using MeSender.Messages.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MeSender.Messages.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessages(this IServiceCollection services, string connectionString, Action<JwtOptions> configureJwtOptions)
    {
        services.AddDbContext<ChatDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IMessageService, MessageService>();
        services.AddSingleton(TimeProvider.System);

        services.Configure(configureJwtOptions);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                };
            });

        services.AddAuthorization();

        return services;
    }
}
