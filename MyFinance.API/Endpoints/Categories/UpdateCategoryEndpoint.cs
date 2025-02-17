using MyFinance.API.Common.Api;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Categories;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Categories;

public class UpdateCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:Guid}", HandleAsync)
        .WithName("Category: Update")
        .WithSummary("Updates category")
        .WithOrder(2)
        .Produces<Response<Category?>>();
    }

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        UpdateCategoryRequest request, 
        ICategoryHandler handler, 
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