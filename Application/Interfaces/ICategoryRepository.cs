using Domain.Entities;

namespace Application.Interfaces;

public interface ICategoryRepository
{
    Task<int> CreateCategoryAsync(Category category);
    Task<int> UpdateCategoryAsync(Category category);
    Task<int> DeleteCategoryAsync(Category category);
    Task<List<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(Guid categoryId);
    Task<int> GetNextSortOrderAsync();
}