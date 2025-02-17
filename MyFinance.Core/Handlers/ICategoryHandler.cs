using MyFinance.Core.Models;
using MyFinance.Core.Requests.Categories;
using MyFinance.Core.Response;

namespace MyFinance.Core.Handlers;

public interface ICategoryHandler
{
    Task<Response<Category?>> CreateAsync(CreateCategoryRequest request);
    Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request);
    Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request);
    Task<PagedResponse<List<Category>>> GetAllAsync(GetAllCategoriesRequest request);
    Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request);
}
