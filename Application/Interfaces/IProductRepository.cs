using Application.DTOs.ProductImage;
using Domain.Entities;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<int> CreateProductAsync(Product product);
    Task<int> UpdateProductAsync(Product product);
    Task<int> DeleteProductAsync(Product product);
    Task<List<Product>> GetProductsAsync();
    Task<Product?> GetProductByIdAsync(Guid productId);
    Task<int> GetNextSortOrderAsync(Guid productId);
    Task<int> AddImageToProductAsync(ProductImage productImage);
    Task<int> DeleteImageFromProductAsync(ProductImage productImage);
}