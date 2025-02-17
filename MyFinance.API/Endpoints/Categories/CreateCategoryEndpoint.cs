using MyFinance.API.Common.Api;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Categories;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Categories;

public class CreateCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", HandleAsync)
        .WithName("Category: Create")
        .WithSummary("Creates new category")
        .WithOrder(1)
        .Produces<Response<Category?>>();
    }

    //IResult
    //generic type for HTTP responses
    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        ICategoryHandler handler, 
        CreateCategoryRequest request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.CreateAsync(request);

        return result.IsSuccess 
            ? TypedResults.Created($"/{result?.Data?.Id}", result)
            : TypedResults.BadRequest(result.Data);
    }
}
