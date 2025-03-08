using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;
using System.Net.Http.Json;

namespace MyFinance.Web.Handlers;

public class VoucherHandler(IHttpClientFactory httpClientFactory) : IVoucherHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);
    public async Task<Response<Voucher?>> GetByNumberAsync(GetVoucherByNumberRequest request)
        => await _client.GetFromJsonAsync<Response<Voucher?>>($"v1/vouchers/{request.Number}")
        ?? new Response<Voucher?>(null, 400, "Unable to find voucher");

}
