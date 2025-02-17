using MyFinance.API.Common.Api;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models.Reports;
using MyFinance.Core.Requests.Reports;
using MyFinance.Core.Response;
using System.Security.Claims;

namespace MyFinance.API.Endpoints.Reports
{
    public class GetExpensesByCategoryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/expenses", HandleAsync)
            .Produces<Response<List<ExpensesByCategory>?>>();

        private static async Task<IResult> HandleAsync(
            ClaimsPrincipal user, 
            IReportHandler handler)
        {
            var request = new GetExpenseByCategoryRequest
            {
                UserId = user.Identity?.Name ?? ""
            };

            request.UserId = user.Identity?.Name ?? string.Empty;
            var result = await handler.GetExpensesByCategoryReportAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
