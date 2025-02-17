using Microsoft.AspNetCore.Mvc;
using MyFinance.API.Common.Api;
using MyFinance.Core;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Categories;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Categories;

public class GetAllCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandleAsync)
            .WithName("Category: Get All")
            .WithSummary("Get all categories")
            .WithOrder(5)
            .Produces<PagedResponse<List<Category>>>();
    }

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        ICategoryHandler handler, 
        [FromQuery]int pgNum = Configuration.DefaultPageNumber,
        [FromQuery]int pgSz = Configuration.DefaultPageSize)
    {
        var request = new GetAllCategoriesRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PageNumber = pgNum,
            PageSize = pgSz
        };

        var result = await handler.GetAllAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
