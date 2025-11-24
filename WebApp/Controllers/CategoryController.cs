using Application.DTOs.Category;
using Application.Interfaces;
using Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCategoryAsync(CreateCategoryDto model)
    {
        var result = await categoryService.CreateCategoryAsync(model);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateCategoryAsync(UpdateCategoryDto model)
    {
        var result = await categoryService.UpdateCategoryAsync(model);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteCategoryAsync(Guid categoryId)
    {
        var result = await categoryService.DeleteCategoryAsync(categoryId);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        var result = await categoryService.GetCategoriesAsync();
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryByIdAsync(Guid categoryId)
    {
        var result = await categoryService.GetCategoryByIdAsync(categoryId);
        return StatusCode(result.StatusCode, result);
    }
}