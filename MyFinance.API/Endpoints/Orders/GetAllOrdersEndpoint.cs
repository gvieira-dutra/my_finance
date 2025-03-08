using Microsoft.AspNetCore.Mvc;
using MyFinance.API.Common.Api;
using MyFinance.Core;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Orders;

public class GetAllOrdersEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
        .WithName("Orders: Get all")
        .WithSummary("Get all orders")
        .WithDescription("Get all orders")
        .WithOrder(5)
        .Produces<PagedResponse<List<Order>?>>();
    private static async Task<IResult> HandleAsync(
        IOrderHandler handler,
        ClaimsPrincipal user,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize)
    {
        var request = new GetAllOrdersRequest
        {
            UserId = user.Identity!.Name ?? string.Empty,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await handler.GetAllAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }

}
