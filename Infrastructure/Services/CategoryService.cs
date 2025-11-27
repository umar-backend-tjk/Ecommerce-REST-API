using System.Net;
using Application.DTOs.Category;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Responses;
using Serilog;

namespace Infrastructure.Services;

public class CategoryService(
    ICategoryRepository categoryRepository, 
    IMapper mapper) : ICategoryService
{
    public async Task<ServiceResult> CreateCategoryAsync(CreateCategoryDto model)
    {
        try
        {
            var categoryEntity = mapper.Map<Category>(model);
            categoryEntity.SortOrder = await categoryRepository.GetNextSortOrderAsync();
            
            var result = await categoryRepository.CreateCategoryAsync(categoryEntity);
            
            if (result == 0)
                return ServiceResult.Fail("Failed to create category");
            
            return ServiceResult.Ok($"Category '{categoryEntity.Name}' created successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in CategoryService.CreateCategoryAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> UpdateCategoryAsync(UpdateCategoryDto model)
    {
        try
        {
            var existingCategory = await categoryRepository.GetCategoryByIdAsync(model.Id);
            if (existingCategory == null) 
                return ServiceResult.Fail("Category not found", HttpStatusCode.NotFound);

            if (model.ParentCategoryId.HasValue)
            {
                var existingParentCategory = await categoryRepository.GetCategoryByIdAsync(model.ParentCategoryId.Value);
                if (existingParentCategory == null)
                    return ServiceResult.Fail("Not found parent-category");
            }

            mapper.Map(model, existingCategory);
            
            existingCategory.UpdatedAt = DateTime.UtcNow;
            
            var result = await categoryRepository.UpdateCategoryAsync(existingCategory);

            if (result == 0)
                return ServiceResult.Fail("Failed to update category");
            
            return ServiceResult.Ok("Category updated successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in CategoryService.UpdateCategoryAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> DeleteCategoryAsync(Guid categoryId)
    {
        try
        {
            var category = await categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null) 
                return ServiceResult.Fail("Category not found", HttpStatusCode.NotFound);
            
            var result = await categoryRepository.DeleteCategoryAsync(category);
            
            if (result == 0) 
                return  ServiceResult.Fail("Failed to delete category");
            
            return ServiceResult.Ok("Category deleted successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in CategoryService.DeleteCategoryAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<List<GetCategoryDto>>> GetCategoriesAsync()
    {
        try
        {
            var categories = await categoryRepository.GetCategoriesAsync();
            if (categories.Count == 0) 
                return ServiceResult<List<GetCategoryDto>>.Fail("No categories found", HttpStatusCode.NotFound);

            var mappedList = mapper.Map<List<GetCategoryDto>>(categories);
            
            return ServiceResult<List<GetCategoryDto>>.Ok(mappedList);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in CategoryService");
            return ServiceResult<List<GetCategoryDto>>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<GetCategoryDto>> GetCategoryByIdAsync(Guid categoryId)
    {
        try
        {
            var category = await categoryRepository.GetCategoryByIdAsync(categoryId);
            
            if (category == null)
                return ServiceResult<GetCategoryDto>.Fail("Category not found", HttpStatusCode.NotFound);
            
            var mappedCategory = mapper.Map<GetCategoryDto>(category);
            
            return ServiceResult<GetCategoryDto>.Ok(mappedCategory);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in CategoryService");
            return ServiceResult<GetCategoryDto>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }
}