using Blazored.LocalStorage;
using ChatRoom;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Polly;
using Polly.Extensions.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthHandler>();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddHttpClient("IdentityApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5244");
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

builder.Services.AddHttpClient("MessagesApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5344");
})
.AddHttpMessageHandler<AuthHandler>()
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MessagesApi"));
builder.Services.AddMudServices();

await builder.Build().RunAsync();
