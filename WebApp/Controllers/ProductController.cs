using Application.DTOs.Product;
using Application.DTOs.ProductDtos;
using Application.DTOs.ProductImage;
using Application.Interfaces;
using Domain.Filters;
using Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost]
    public async Task<ServiceResult> CreateProductAsync(CreateProductDto model)
        => await productService.CreateProductAsync(model);

    [HttpPut]
    public async Task<ServiceResult> UpdateProductAsync(UpdateProductDto model)
        => await productService.UpdateProductAsync(model);
    
    [HttpDelete("{id}")]
    public async Task<ServiceResult> DeleteProductAsync(Guid id)
        => await productService.DeleteProductAsync(id);

    [HttpGet]
    public async Task<ServiceResult<List<GetProductDto>>> GetProductsAsync([FromQuery] ProductFilter filter)
        => await productService.GetProductsAsync(filter);
    
    [HttpGet("{id}")]
    public async Task<ServiceResult<GetProductDto>> GetProductByIdAsync(Guid id)
        => await productService.GetProductByIdAsync(id);

    [HttpPost("{id}/images")]
    public async Task<ServiceResult> AddImageToProductAsync(AddProductImageDto imageDto)
        => await productService.AddImageToProductAsync(imageDto);
    
    [HttpDelete("{id}/images/{imageId}")]
    public async Task<ServiceResult> RemoveImageFromProductAsync(Guid productId, Guid imageId)
        => await productService.RemoveImageFromProductAsync(productId, imageId);
}