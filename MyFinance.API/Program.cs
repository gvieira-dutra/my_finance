using MyFinance.API;
using MyFinance.API.Common.Api;
using MyFinance.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfigurations();
builder.AddSecurity();
builder.AddDataContexts();
await builder.SeedMockDataAsync();
builder.AddCrossOrigin();
builder.AddSwagger();
builder.AddServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.SwaggerDev();
}

app.UseCors(ApiConfiguration.CorsPolicyName);
app.UseSecurity();
app.MapEndpoints();

app.Run();
