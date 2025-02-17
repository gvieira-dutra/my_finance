using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Categories;
using MyFinance.Core.Response;
using System.Net.Http.Json;

namespace MyFinance.Web.Handlers;

public class CategoryHandler(IHttpClientFactory httpClientFactory) : ICategoryHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

    public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
    {
        var result = await _client.PostAsJsonAsync("v1/categories", request);

        return await result.Content.ReadFromJsonAsync<Response<Category?>>()
        ?? new Response<Category?>(null, 400, "Fail to create category");
    }

    public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
    {
        var result = await _client.DeleteAsync($"v1/categories/{request.Id}");

        return await result.Content.ReadFromJsonAsync<Response<Category?>>()
        ?? new Response<Category?>(null, 400, "Fail to delete category");
    }

    public async Task<PagedResponse<List<Category>>> GetAllAsync(GetAllCategoriesRequest request)
    => await _client.GetFromJsonAsync<PagedResponse<List<Category>>>("v1/categories")
       ?? new PagedResponse<List<Category>>(null, 400, "Unable to retrieve categories");

    public async Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
    {
        Console.WriteLine("INSIDE HANDLER");

        var result = await _client.GetFromJsonAsync<Response<Category?>>($"v1/categories/{request.Id}");
        if(result != null)
        {
            Console.WriteLine($"RESULT {result}");
            Console.WriteLine($"RESULT.DATA {result.Data}");
            Console.WriteLine($"RESULT.MESSAGE {result.Message}");
            return result;
        }
        else
        {
            Console.WriteLine("RESULT NULL");
            return new Response<Category?>(null, 400, "Fail to retrieve category");
        }

    }

    public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
    {
        var result = await _client.PutAsJsonAsync($"v1/categories/{request.Id}", request);

        return await result.Content.ReadFromJsonAsync<Response<Category?>>()
        ?? new Response<Category?>(null, 400, "Fail to update category");
    }
}
