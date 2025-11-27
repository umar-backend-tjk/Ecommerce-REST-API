using Application.DTOs.ProductImage;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(DataContext context) : IProductRepository
{
    public async Task<int> CreateProductAsync(Product product)
    {
        await context.Products.AddAsync(product);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateProductAsync(Product product)
    {
        context.Entry(product).State = EntityState.Modified;
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteProductAsync(Product product)
    {
        context.Products.Remove(product);
        return await context.SaveChangesAsync();
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        return await context.Products
            .Include(p => p.Images)
            .Where(p => p.IsActive).ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId)
    {
        return await context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);
    }

    public async Task<int> GetNextSortOrderAsync(Guid productId)
    {
        int? maxSortOrder = await context.ProductImages
            .Where(p => p.ProductId == productId)
            .MaxAsync(c => (int?)c.SortOrder);

        return (maxSortOrder ?? 0) + 1;
    }

    public async Task<int> AddImageToProductAsync(ProductImage productImage)
    {
        await context.ProductImages.AddAsync(productImage);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteImageFromProductAsync(ProductImage productImage)
    {
        context.ProductImages.Remove(productImage);
        return await context.SaveChangesAsync();
    }
}