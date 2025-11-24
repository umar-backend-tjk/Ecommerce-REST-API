using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(DataContext context) : ICategoryRepository
{
    public async Task<int> CreateCategoryAsync(Category category)
    {
        await context.Categories.AddAsync(category);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateCategoryAsync(Category category)
    {
        context.Categories.Update(category);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteCategoryAsync(Category category)
    {
        category.IsActive = false;
        return await context.SaveChangesAsync();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
    {
        return await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId && c.IsActive);
    }

    public async Task<int> GetNextSortOrderAsync()
    {
        int? maxSortOrder = await context.Categories
            .MaxAsync(c => (int?)c.SortOrder);

        return (maxSortOrder ?? 0) + 1;
    }

}