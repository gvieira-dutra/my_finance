using Microsoft.EntityFrameworkCore;
using MyFinance.API.Data;
using MyFinance.Core.Handlers;
using MyFinance.Core.Models;
using MyFinance.Core.Requests.Categories;
using MyFinance.Core.Response;

namespace MyFinance.API.Handlers;

public class CategoryHandler(AppDbContext context) : ICategoryHandler
{
    public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
    {
        try
        {
            var category = new Category
            {
                UserId = request.UserId,
                Title = request.Title,
                Description = request.Description
            };

            await context.AddAsync(category);
            await context.SaveChangesAsync();

            return new Response<Category?>(category, 200, "[API007] Category created successfully");
        }
        catch (Exception)
        {
            return new Response<Category?>(null, 500, "[API001] Failed to create category");
        }
    }

    //AsNoTracking and FirstOrDefaultAsync 
    //is provided by package Microsoft.EntityFrameworkCore
    public async Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
    {
        try
        {
            var category = await context
                .Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            return category is null
                ? new Response<Category?>(null, 404, "[API009] Category not found")
                : new Response<Category?>(category, 200, "[API010] Category retrieved successfully");
        }
        catch (Exception)
        {
            return new Response<Category?>(null, 500, "[API011]Error while retrieving category");
        }
    }

    public async Task<PagedResponse<List<Category>>> GetAllAsync(GetAllCategoriesRequest request)
    {
        try
        {
            var query = context
                .Categories
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId)
                .OrderBy(x => x.Title);

            var categories = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query
                .CountAsync();

            return new PagedResponse<List<Category>>(
                categories,
                count,
                request.PageNumber,
                request.PageSize
                );
        }
        catch (Exception)
        {
            return new PagedResponse<List<Category>>(
                null,
                500,
                "Unable to retrieve categories"
                );
        }
    }

    public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
    {
        try
        {
            var category = await context
                .Categories
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (category is null) return new Response<Category?>(null, 404, "[API008] Category not found");

            category.Title = request.Title;
            category.Description = request.Description;

            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return new Response<Category?>(category, 201, "[API002] Category updated successfully");
        }
        catch (Exception)
        {
            return new Response<Category?>(null, 500, "[API003]Error while updating category");
        }
    }

    public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
    {
        try
        {
            var category = await context
                .Categories
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (category is null) return new Response<Category?>(null, 404, "[API004] Category not found");

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return new Response<Category?>(category, 200, "[API005] Category deleted successfully");
        }
        catch (Exception)
        {
            return new Response<Category?>(null, 500, "[API006] Error while deleting category");
        }
    }
}
