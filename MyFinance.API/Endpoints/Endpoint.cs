using MyFinance.API.Common.Api;
using MyFinance.API.Endpoints.Categories;
using MyFinance.API.Endpoints.Identity;
using MyFinance.API.Endpoints.Orders;
using MyFinance.API.Endpoints.Reports;
using MyFinance.API.Endpoints.Stripe;
using MyFinance.API.Endpoints.Transactions;
using MyFinance.API.Models;

namespace MyFinance.API.Endpoints;

public static class Endpoint
{
    // Extension Method
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app
            .MapGroup("");

        endpoints.MapGroup("/")
            .WithTags("HealthCheck")
            .MapGet("/", () => new { message = "OK" });

        endpoints.MapGroup("v1/categories")
            .WithTags("Categories")
            .RequireAuthorization()
            .MapEndpoint<CreateCategoryEndpoint>()
            .MapEndpoint<UpdateCategoryEndpoint>()
            .MapEndpoint<DeleteCategoryEndpoint>()
            .MapEndpoint<GetCategoryByIdEndpoint>()
            .MapEndpoint<GetAllCategoryEndpoint>();

        endpoints.MapGroup("v1/transactions")
            .WithTags("Transactions")
            .RequireAuthorization()
            .MapEndpoint<CreateTransactionEndpoint>()
            .MapEndpoint<UpdateTransactionEndpoint>()
            .MapEndpoint<DeleteTransactionEndpoint>()
            .MapEndpoint<GetTransactionByIdEndpoint>()
            .MapEndpoint<GetTrasactionByPeriodEndpoint>();

        endpoints.MapGroup("/v1/reports")
            .WithTags("Reports")
            .RequireAuthorization()
            .MapEndpoint<GetExpensesByCategoryEndpoint>()
            .MapEndpoint<GetFinancialSummaryEndpoint>()
            .MapEndpoint<GetIncomesAndExpensesEndpoint>()
            .MapEndpoint<GetIncomesByCategoryEndpoint>();

        endpoints.MapGroup("/v1/products")
            .WithTags("Products")
            .MapEndpoint<GetAllProductsEndpoint>()
            .MapEndpoint<GetProductBySlugEndpoint>();

        endpoints.MapGroup("/v1/payments-stripe")
            .WithTags("Payments - Stripe")
            .RequireAuthorization()
            .MapEndpoint<CreateSessionEndpoint>();

        endpoints.MapGroup("v1/orders")
            .WithTags("Orders")
            .RequireAuthorization()
            .MapEndpoint<CancelOrderEndpoint>()
            .MapEndpoint<CreateOrderEndpoint>()
            .MapEndpoint<GetAllOrdersEndpoint>()
            .MapEndpoint<GetOrderByNumberEndpoint>()
            .MapEndpoint<PayOrderEndpoint>()
            .MapEndpoint<RefundOrderEndpoint>();

        endpoints.MapGroup("v1/vouchers")
            .WithTags("Voucher")
            .RequireAuthorization()
            .MapEndpoint<GetVoucherByNumberEndpoint>();

        endpoints.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapIdentityApi<User>();

        endpoints.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapEndpoint<LogoutEndpoint>()
            .MapEndpoint<GetRolesEndpoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
