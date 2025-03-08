using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;
using System.Net.Http.Json;

namespace MyFinance.Web.Handlers;

public class OrderHandler(IHttpClientFactory httpClientFactory) : IOrderHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

    public async Task<Response<Order?>> CancelAsync(CancelOrderRequest request)
    {
        var result = await _client.PostAsJsonAsync($"v1/orders/{request.Id}/cancel", request);

        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
            ?? new Response<Order?>(null, 400, "Unable to cancel order");
    }

    public async Task<Response<Order?>> CreateAsync(CreateOrderRequest request)
    {
        var result = await _client.PostAsJsonAsync($"v1/orders", request);

        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
            ?? new Response<Order?>(null, 400, "Unable to create order");
    }

    public async Task<PagedResponse<List<Order>?>> GetAllAsync(GetAllOrdersRequest request)
        => await _client.GetFromJsonAsync<PagedResponse<List<Order>?>>("v1/orders")
        ?? new PagedResponse<List<Order>?>(null, 400, "Unable to retrieve orders");


    public async Task<Response<Order?>> GetByNumberAsync(GetOrderByNumberRequest request)
        => await _client.GetFromJsonAsync<Response<Order?>>($"v1/orders/{request.Number}")
        ?? new Response<Order?>(null, 400, "Unable to retrieve order");

    public async Task<Response<Order?>> PayAsync(PayOrderRequest request)
    {
        var result = await _client.PostAsJsonAsync($"v1/orders/{request.Number}", request);

        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
            ?? new Response<Order?>(null, 400, "Unable to pay order");
    }

    public async Task<Response<Order?>> RefundAsync(RefundOrderRequest request)
    {
        var result = await _client.PostAsJsonAsync($"v1/orders/{request.Id}/refund", request);

        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
            ?? new Response<Order?>(null, 400, "Unable to refund order");
    }
}
