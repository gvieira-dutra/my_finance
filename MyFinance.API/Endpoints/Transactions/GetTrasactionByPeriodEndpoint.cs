using Microsoft.AspNetCore.Mvc;
using MyFinance.API.Common.Api;
using MyFinance.API.Models;
using MyFinance.Core;
using MyFinance.Core.Common.Extensions;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Transactions;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Transactions;

public class GetTrasactionByPeriodEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandleAsync)
            .WithName("Transactions: Get by period")
            .WithSummary("Get transactions by period")
            .WithOrder(5)
            .Produces<PagedResponse<List<Transaction?>>>();
    }

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        ITransactionHandler handler,
        [FromQuery] int pgNum = Configuration.DefaultPageNumber,
        DateTime? endDate = null, 
        DateTime? startDate = null,
        [FromQuery] int pgSz = Configuration.DefaultPageSize)
    {
        var request = new GetTransactionByPeriodRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PageNumber = pgNum,
            PageSize = pgSz,
            StartDate = startDate ?? DateTime.Now.GetFirstDay(),
            EndDate = endDate ?? DateTime.Now.GetLastDay()
        };

        var result = await handler.GetByPeriodAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
