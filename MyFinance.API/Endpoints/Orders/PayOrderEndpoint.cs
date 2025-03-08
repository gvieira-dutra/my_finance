using MyFinance.API.Common.Api;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Orders;

public class PayOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/{number}", HandleAsync)
        .WithName("Orders: Pay")
        .WithSummary("Pay for an order")
        .WithDescription("Pay for an order")
        .Produces<Response<Order?>>();

    private static async Task<IResult> HandleAsync(
        IOrderHandler handler,
        ClaimsPrincipal user,
        PayOrderRequest request,
        string number)
    {
        request.Number = number;
        request.UserId = user.Identity!.Name ?? string.Empty;

        var response = await handler.PayAsync(request);

        return response.IsSuccess
            ? TypedResults.Ok(response)
            : TypedResults.BadRequest(response);
    }
}
