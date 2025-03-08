using MyFinance.API.Common.Api;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Orders;

public class RefundOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/{id}/refund", HandleAsync)
        .WithName("Orders: Refund an order")
        .WithSummary("Refund an order")
        .WithDescription("Refund an order")
        .Produces<Response<Order?>>();
    private static async Task<IResult> HandleAsync(
        IOrderHandler handler,
        ClaimsPrincipal user,
        long id)
    {
        var request = new RefundOrderRequest
        {
            Id = id,
            UserId = user.Identity!.Name ?? string.Empty
        };

        var result = await handler.RefundAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
