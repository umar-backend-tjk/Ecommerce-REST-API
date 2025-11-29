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
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost]
    public async Task<ServiceResult> CreateProductAsync(CreateProductDto model)
        => await productService.CreateProductAsync(model);

    [HttpPatch]
    public async Task<ServiceResult> UpdateProductAsync(UpdateProductDto model)
        => await productService.UpdateProductAsync(model);
    
    [HttpDelete("{id}")]
    public async Task<ServiceResult> DeleteProductAsync(Guid id)
        => await productService.DeleteProductAsync(id);

    [HttpGet]
    public async Task<ServiceResult<List<GetProductDto>>> GetProductsAsync([FromQuery] ProductFilter filter)
        => await productService.GetProductsAsync(filter);
    
    [HttpGet("{slug}")]
    public async Task<ServiceResult<GetProductDto>> GetProductBySlugAsync(string slug)
        => await productService.GetProductBySlugAsync(slug);

    [HttpPost("{id}/images")]
    public async Task<ServiceResult> AddImageToProductAsync(Guid id, AddProductImageDto imageDto)
        => await productService.AddImageToProductAsync(id, imageDto);
    
    [HttpDelete("{id}/images/{imageId}")]
    public async Task<ServiceResult> RemoveImageFromProductAsync(Guid productId, Guid imageId)
        => await productService.RemoveImageFromProductAsync(productId, imageId);

    [Authorize]
    [HttpPost("{id}/reviews")]
    public async Task<ServiceResult> AddReviewToProductAsync(Guid id, int stars)
        => await productService.AddReviewToProductAsync(id, stars);
    
    [Authorize]
    [HttpPut("{id}/reviews")]
    public async Task<ServiceResult> UpdateReviewOfProductAsync(Guid id, int stars)
        => await productService.UpdateReviewOfProductAsync(id, stars);
    
    [Authorize]
    [HttpDelete("{id}/reviews/{reviewId}")]
    public async Task<ServiceResult> DeleteReviewFromProductAsync(Guid id, Guid reviewId)
        => await productService.DeleteReviewFromProductAsync(id, reviewId);
}