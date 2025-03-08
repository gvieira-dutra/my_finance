using MyFinance.Core.Requests.Stripe;
using MyFinance.Core.Response;

namespace MyFinance.Core.Handlers;
public interface IStripeHandler
{
    Task<Response<string?>> CreateSessionAsync(CreateSessionRequest request);
    Task<Response<List<StripeTransactionResponse>>> GetTransactionsByOrderNumberAsync(GetTransactionsByOrderNumberRequest request);
}

