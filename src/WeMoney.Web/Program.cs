using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using WeMoney.Web;
using WeMoney.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
                 ?? throw new InvalidOperationException("ApiBaseUrl não configurada em wwwroot/appsettings.json.");

// ApiBaseUrl pode ser absoluta (dev) ou relativa ao próprio site (Docker, via proxy reverso).
var apiBaseAddress = new Uri(new Uri(builder.HostEnvironment.BaseAddress), apiBaseUrl);
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = apiBaseAddress });
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<TokenStorage>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddScoped<ApiClient>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
