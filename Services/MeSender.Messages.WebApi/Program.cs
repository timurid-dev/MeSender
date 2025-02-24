var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
var app = builder.Build();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.MapControllers();
await app.RunAsync();
