using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;
using System.Net.Http.Json;

namespace MyFinance.Web.Handlers;

public class ProductHandler(IHttpClientFactory httpClientFactory) : IProductHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

    public async Task<PagedResponse<List<Product>?>> GetAllAsync(GetAllProductsRequest request)
        => await _client.GetFromJsonAsync<PagedResponse<List<Product>?>>("v1/products")
        ?? new PagedResponse<List<Product>?>(null, 400, "Unable to find products");

    public async Task<Response<Product?>> GetBySlugAsync(GetProductBySlugRequest request)
    => await _client.GetFromJsonAsync<Response<Product?>>($"v1/products/{request.Slug}")
        ?? new Response<Product?>(null, 400, "Unable to find product");
}
