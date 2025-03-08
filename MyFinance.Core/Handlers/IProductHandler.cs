using MyFinance.Core.Models;
using MyFinance.Core.Requests.Orders;
using MyFinance.Core.Response;

namespace MyFinance.Core.Handlers;
public interface IProductHandler
{
    Task<PagedResponse<List<Product>?>> GetAllAsync(GetAllProductsRequest request);
    Task<Response<Product?>> GetBySlugAsync(GetProductBySlugRequest request);
}
