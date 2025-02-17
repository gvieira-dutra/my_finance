using MyFinance.API.Common.Api;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Transactions;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Transactions;

public class UpdateTransactionEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:Guid}", HandleAsync)
        .WithName("Transaction: Update")
        .WithSummary("Updates transaction")
        .WithOrder(2)
        .Produces<Response<Transaction?>>();
    }

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        UpdateTransactionRequest request,
        ITransactionHandler handler,
        Guid id)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        request.Id = id;

        var result = await handler.UpdateAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result.Data)
            : TypedResults.BadRequest(result.Data);
    }
}
