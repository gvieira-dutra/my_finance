using MyFinance.API.Common.Api;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Orders;

public class GetOrderByNumberEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{number}", HandleAsync)
        .WithName("Order: Get by number")
        .WithSummary("Gets order by number")
        .WithDescription("Gets order by number")
        .Produces<Response<Order?>>();

    private static async Task<IResult> HandleAsync(
       IOrderHandler handler,
       ClaimsPrincipal user,
       string number)
    {
        var request = new GetOrderByNumberRequest
        {
            Number = number,
            UserId = user.Identity!.Name ?? string.Empty
        };

        var result = await handler.GetByNumberAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
