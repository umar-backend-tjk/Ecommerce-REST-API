using Application.DTOs.Product;
using Application.DTOs.ProductDtos;
using Application.DTOs.ProductImage;
using Domain.Filters;
using Domain.Responses;

namespace Application.Interfaces;

public interface IProductService
{
    Task<ServiceResult> CreateProductAsync(CreateProductDto model);
    Task<ServiceResult> UpdateProductAsync(UpdateProductDto model);
    Task<ServiceResult> DeleteProductAsync(Guid productId);
    Task<ServiceResult<List<GetProductDto>>> GetProductsAsync(ProductFilter filter);
    Task<ServiceResult<GetProductDto>> GetProductByIdAsync(Guid productId);
    Task<ServiceResult> AddImageToProductAsync(AddProductImageDto productImage);
    Task<ServiceResult> RemoveImageFromProductAsync(Guid productId, Guid imageId);
}