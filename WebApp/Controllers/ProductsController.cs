using Application.DTOs.ProductDtos;
using Application.DTOs.ProductImage;
using Application.Interfaces;
using Domain.Filters;
using Domain.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(CreateProductDto model)
    {
        var result = await productService.CreateProductAsync(model);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateProductAsync(UpdateProductDto model)
    {
        var result = await productService.UpdateProductAsync(model);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductAsync(Guid id)
    {
        var result = await productService.DeleteProductAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<GetProductDto>>> GetProductsAsync([FromQuery] ProductFilter filter)
    {
        var result = await productService.GetProductsAsync(filter);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpGet("{slug}")]
    public async Task<ActionResult<GetProductDto>> GetProductBySlugAsync(string slug)
    {
        var result = await productService.GetProductBySlugAsync(slug);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImageToProductAsync(Guid id, AddProductImageDto imageDto)
    {
        var result = await productService.AddImageToProductAsync(id, imageDto);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpDelete("{id}/images/{imageId}")]
    public async Task<IActionResult> RemoveImageFromProductAsync(Guid productId, Guid imageId)
    {
        var result = await productService.RemoveImageFromProductAsync(productId, imageId);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpPost("{id}/reviews")]
    public async Task<IActionResult> AddReviewToProductAsync(Guid id, int stars)
    {
        var result = await productService.AddReviewToProductAsync(id, stars);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpPut("{id}/reviews")]
    public async Task<IActionResult> UpdateReviewOfProductAsync(Guid id, int stars)
    {
        var result = await productService.UpdateReviewOfProductAsync(id, stars);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpDelete("{id}/reviews/{reviewId}")]
    public async Task<IActionResult> DeleteReviewFromProductAsync(Guid id, Guid reviewId)
    {
        var result = await productService.DeleteReviewFromProductAsync(id, reviewId);
        return StatusCode(result.StatusCode, result);
    }
}