using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyFinance.API.Data;
using MyFinance.API.Handlers;
using MyFinance.API.Models;
using MyFinance.Core;
using MyFinance.Core.Handlers;

namespace MyFinance.API.Common.Api;

public static class BuilderExtension 
{
    public static void AddConfigurations(this WebApplicationBuilder builder)
    {
        //Connection string is coming from user-secrets
        Configuration.ConnectionStr = builder
            .Configuration
            .GetConnectionString("DefaultConnection") ?? string.Empty;

        Configuration.BackEndURL = builder.Configuration.GetValue<string>("BackEndURL") ?? string.Empty;

        Configuration.FrontEndURL = builder.Configuration.GetValue<string>("FrontEndURL") ?? string.Empty;
    }

    public static void AddSwagger(this WebApplicationBuilder builder)
    {

        builder.Services.AddEndpointsApiExplorer();

        //AddSwagerGen provided by package SwashBuckle.AspNetCore
        builder.Services.AddSwaggerGen(x =>
        {
            x.CustomSchemaIds(o => o.FullName);
        });
    }

    public static void AddSecurity(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();
         
        builder.Services.AddAuthorization();
    }

    public static void AddDataContexts(this WebApplicationBuilder builder)
    {
        // AddDbContext
        //is provided by package Microsoft.EntityFrameworkCore.SqlServer
        builder.Services.AddDbContext<AppDbContext>(x =>
        {
            x.UseSqlServer(Configuration.ConnectionStr);
        });


        builder.Services
            .AddIdentityCore<User>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddApiEndpoints();
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddTransient<ICategoryHandler, CategoryHandler>()
            .AddTransient<ITransactionHandler, TransactionHandler>()
            .AddTransient<IReportHandler, ReportsHandler>();
    }

    public static void AddCrossOrigin(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(
            options => options.AddPolicy(
                ApiConfiguration.CorsPolicyName,
                policy => policy
                .WithOrigins([
                    Configuration.BackEndURL,
                    Configuration.FrontEndURL
                    ])
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                ));
    }
}
