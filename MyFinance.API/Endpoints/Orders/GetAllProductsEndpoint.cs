using Microsoft.AspNetCore.Mvc;
using MyFinance.API.Common.Api;
using MyFinance.Core;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Orders;

public class GetAllProductsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
       => app.MapGet("/", HandleAsync)
       .WithName("Products: Get all")
       .WithSummary("Get all products")
       .WithDescription("Get all products")
       .WithOrder(4)
       .Produces<PagedResponse<List<Product>?>>();
    private static async Task<IResult> HandleAsync(
        IProductHandler handler,
        ClaimsPrincipal user,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize)
    {
        var request = new GetAllProductsRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await handler.GetAllAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }

}
