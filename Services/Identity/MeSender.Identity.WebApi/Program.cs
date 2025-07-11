using Hangfire;
using MeSender.Identity.Data;
using MeSender.Identity.Extensions;
using MeSender.Identity.Models;
using MeSender.Identity.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException();
builder.Services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString));
builder.Services.AddHostedService<MigrationService>();
builder.Services.AddUsers(options => builder.Configuration.GetSection(JwtOptions.SectionName).Bind(options));
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
    .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase; });
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MeSender.Identity.WebApi", Version = "v1",
    });
});

var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException();
builder.Services.AddHangfireEntity(redisConnectionString);

var app = builder.Build();
app.UseHangfireDashboard();
app.UseSwagger();
app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "MeSender.Identity.WebApi v1"));
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();

public abstract partial class Program;
