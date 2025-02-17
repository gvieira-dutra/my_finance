using MyFinance.API.Common.Api;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Identity;

public class GetRolesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)

        //By using claimsPrincipal, we'll retrieve info
        //from cookies and don't need to go to db every time. 
        => app.MapGet("/roles", Handle)
            .RequireAuthorization();

    private static Task<IResult> Handle(ClaimsPrincipal user)
    {
        if (user.Identity is null || !user.Identity.IsAuthenticated)
            return Task.FromResult(Results.Unauthorized());

        var identity = (ClaimsIdentity)user.Identity;
        var roles = identity
            .FindAll(identity.RoleClaimType)
            .Select(c => new RoleClaim
            {
                Issuer = c.Issuer,
                OriginalIssuer = c.OriginalIssuer,
                Type = c.Type,
                Value = c.Value,
                ValueType = c.ValueType
            });

        return Task.FromResult<IResult>(TypedResults.Json(roles));
    }
}

