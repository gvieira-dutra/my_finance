using MyFinance.API.Common.Api;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;

namespace MyFinance.API.Endpoints.Orders;

public class GetVoucherByNumberEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{number}", HandleAsync)
        .WithName("Voucher: Get by number")
        .WithSummary("Gets voucher by number")
        .WithDescription("Gets voucher by number")
        .Produces<Response<Voucher?>>();

    private static async Task<IResult> HandleAsync(
        IVoucherHandler handler,
        string number)
    {
        var request = new GetVoucherByNumberRequest
        {
            Number = number
        };

        var result = await handler.GetByNumberAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
