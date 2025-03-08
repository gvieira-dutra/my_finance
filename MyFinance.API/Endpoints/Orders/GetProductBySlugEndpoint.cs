using MyFinance.API.Common.Api;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;

namespace MyFinance.API.Endpoints.Orders;

public class GetProductBySlugEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{slug}", HandleAsync)
        .WithName("Product: By slug")
        .WithSummary("Get product by slug")
        .WithDescription("Get product by slug")
        .Produces<Response<Product?>>();

    private static async Task<IResult> HandleAsync(
        string slug,
        IProductHandler handler)
    {
        var request = new GetProductBySlugRequest
        {
            Slug = slug
        };

        var response = await handler.GetBySlugAsync(request);

        return response.IsSuccess
            ? TypedResults.Ok(response)
            : TypedResults.BadRequest(response);
    }
}
