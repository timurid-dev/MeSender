using MassTransit;
using MeSender.Messages.Extensions;
using MeSender.Messages.WebApi.Extensions;
using MeSender.Messages.WebApi.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
builder.Services.AddMessages(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException());

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MeSender.Messages.WebApi", Version = "v1",
    });
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserLoggedInConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
        {
            h.Username("admin");
            h.Password("password");
        });
        cfg.ReceiveEndpoint("user-loggedin-queue", e =>
        {
            e.ConfigureConsumer<UserLoggedInConsumer>(context);
        });
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "MeSender.Messages.WebApi v1"));
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
