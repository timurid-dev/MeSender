using MeSender.Messages.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
builder.Services.AddMessages(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException());

var app = builder.Build();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.MapControllers();
await app.RunAsync();
