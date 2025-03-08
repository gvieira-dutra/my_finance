using MyFinance.API.Common.Api;
using MyFinance.Core.Handlers;
using MyFinance.Core.Requests.Stripe;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Stripe;

public class CreateSessionEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/session", HandleAsync)
        .WithName("Stripe: Create Session")
        .WithSummary("Creates stripe session")
        .WithDescription("Creates stripe session")
        .Produces<string?>();

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        IStripeHandler handler,
        CreateSessionRequest request)
    {
        request.UserId = user.Identity!.Name ?? string.Empty;

        var result = await handler.CreateSessionAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
