using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MyFinance.Core.Handlers;
using MyFinance.Web;
using MyFinance.Web.Handlers;
using MyFinance.Web.Security;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

Configuration.BackEndUrl = builder.Configuration.GetValue<string>("BackendUrl") ?? string.Empty;

Configuration.StripePublicKey = builder.Configuration.GetValue<string>("StripePublicKey") ?? string.Empty;


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

builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
builder.Services.AddTransient<IAccountHandler, AccountHandler>();
builder.Services.AddTransient<IOrderHandler, OrderHandler>();
builder.Services.AddTransient<IProductHandler, ProductHandler>();
builder.Services.AddTransient<IVoucherHandler, VoucherHandler>();
builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();
builder.Services.AddTransient<IReportHandler, ReportHandler>();
builder.Services.AddTransient<IStripeHandler, StripeHandler>();

builder.Services.AddLocalization();
await builder.Build().RunAsync();
