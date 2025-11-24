using Application.DTOs.Category;
using Domain.Entities;
using Domain.Responses;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult> CreateCategoryAsync(CreateCategoryDto category);
    Task<ServiceResult> UpdateCategoryAsync(UpdateCategoryDto category);
    Task<ServiceResult> DeleteCategoryAsync(Guid categoryId);
    Task<ServiceResult<List<GetCategoryDto>>> GetCategoriesAsync();
    Task<ServiceResult<GetCategoryDto>> GetCategoryByIdAsync(Guid categoryId);
}