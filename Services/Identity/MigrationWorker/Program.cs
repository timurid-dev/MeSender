using MigrationWorker;

var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException();
builder.Services.AddHostedService(provider => new Worker(provider.GetRequiredService<ILogger<Worker>>(), connectionString));

var host = builder.Build();
await host.RunAsync();
