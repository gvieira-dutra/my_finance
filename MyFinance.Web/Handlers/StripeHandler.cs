using MyFinance.Core.Handlers;
using MyFinance.Core.Requests.Stripe;
using MyFinance.Core.Response;
using System.Net.Http.Json;

namespace MyFinance.Web.Handlers;

public class StripeHandler(IHttpClientFactory client) : IStripeHandler
{
    private readonly HttpClient _client = client.CreateClient(Configuration.HttpClientName);
    public async Task<Response<string?>> CreateSessionAsync(CreateSessionRequest request)
    {
        var result = await _client.PostAsJsonAsync("v1/payments-stripe/session", request);
        return await result.Content.ReadFromJsonAsync<Response<string?>>()
            ?? new Response<string?>(null, 400, "Fail to create stripe sessoin");
    }

    public async Task<Response<List<StripeTransactionResponse>>> GetTransactionsByOrderNumberAsync(GetTransactionsByOrderNumberRequest request)
    {
        var result = await _client.PostAsJsonAsync($"v1/payments/stripe/{request.Number}/transactions", request);

        return await result.Content.ReadFromJsonAsync<Response<List<StripeTransactionResponse>>>()
            ?? new Response<List<StripeTransactionResponse>>(null, 500, "Unable to get stripe transactions");
    }
}
