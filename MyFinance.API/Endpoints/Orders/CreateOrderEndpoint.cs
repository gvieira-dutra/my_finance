using MyFinance.API.Common.Api;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Orders;

public class CreateOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
        .WithName("Orders: Creates order")
        .WithSummary("Creates a new order")
        .WithDescription("Creates a new order")
        .WithOrder(1)
        .Produces<Response<Order?>>();
    private static async Task<IResult> HandleAsync(
        IOrderHandler handler,
        CreateOrderRequest request,
        ClaimsPrincipal user)
    {
        request.UserId = user.Identity!.Name ?? string.Empty;


        var result = await handler.CreateAsync(request);
        return result.IsSuccess
            ? TypedResults.Created($"v1/orders/{result.Data?.Number}", result)
            : TypedResults.BadRequest(result);
    }

}
