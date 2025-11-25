using Application.DTOs.Product;
using Domain.Responses;

namespace Application.Interfaces;

public interface IProductService
{
    Task<ServiceResult> CreateProductAsync(CreateProductDto model);
    Task<ServiceResult> UpdateProductAsync(UpdateProductDto model);
    Task<ServiceResult> DeleteProductAsync(Guid productId);
    Task<ServiceResult<List<GetProductDto>>> GetProductsAsync();
    Task<ServiceResult<GetProductDto>> GetProductByIdAsync(Guid productId);
}