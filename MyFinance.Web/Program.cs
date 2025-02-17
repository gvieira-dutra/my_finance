using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyFinance.Web;
using MudBlazor.Services;
using MyFinance.Web.Security;
using Microsoft.AspNetCore.Components.Authorization;
using MyFinance.Core.Handlers;
using MyFinance.Web.Handlers;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

Configuration.BackEndUrl = builder.Configuration.GetValue<string>("BackendUrl") ?? string.Empty;

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<CookieHandler>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

builder.Services.AddScoped(x => (ICookieAuthenticationStateProvider)x.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddMudServices();

builder.Services.AddHttpClient(Configuration.HttpClientName, opt =>
{
    opt.BaseAddress = new Uri(Configuration.BackEndUrl);
}).AddHttpMessageHandler<CookieHandler>();

builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();
builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
builder.Services.AddTransient<IAccountHandler, AccountHandler>();
builder.Services.AddTransient<IReportHandler, ReportHandler>();

builder.Services.AddLocalization();
await builder.Build().RunAsync();
