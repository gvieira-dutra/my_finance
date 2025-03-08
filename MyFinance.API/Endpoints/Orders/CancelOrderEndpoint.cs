using MyFinance.API.Common.Api;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Orders;

public class CancelOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/{id}/cancel", HandleAsync)
        .WithName("Orders: Cancel order")
        .WithSummary("Cancel an order")
        .WithDescription("Cancel an order")
        .WithOrder(2)
        .Produces<Response<Order?>>();
    private static async Task<IResult> HandleAsync(
        IOrderHandler handler,
        long id,
        ClaimsPrincipal user)
    {
        var request = new CancelOrderRequest
        {
            Id = id,
            UserId = user.Identity!.Name ?? string.Empty
        };

        var result = await handler.CancelAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
