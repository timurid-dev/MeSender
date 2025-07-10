using MeSender.Messages.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration["Jwt:Secret"] = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "your-super-secret-key-with-minimum-32-characters";
builder.Configuration["Jwt:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "MeSender";
builder.Configuration["Jwt:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "MeSender";

builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
builder.Services.AddMessages(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException(),
    options => builder.Configuration.GetSection("Jwt").Bind(options));

var app = builder.Build();

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
