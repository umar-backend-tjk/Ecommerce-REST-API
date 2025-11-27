using Application.DTOs.ProductDtos;
using Application.DTOs.ProductImage;
using Application.DTOs.ReviewDtos;
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
    Task<ServiceResult> AddImageToProductAsync(Guid productId, AddProductImageDto productImage);
    Task<ServiceResult> RemoveImageFromProductAsync(Guid productId, Guid imageId);
    Task<ServiceResult> AddReviewToProductAsync(Guid productId, int stars);
    Task<ServiceResult> UpdateReviewOfProductAsync(Guid productId, int stars);
    Task<ServiceResult> DeleteReviewFromProductAsync(Guid productId, Guid reviewId);
}