using System.Net;
using System.Security.Claims;
using Application.DTOs.Product;
using Application.DTOs.ProductDtos;
using Application.DTOs.ProductImage;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Caching;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Infrastructure.Services;

public class ProductService(
    IProductRepository productRepository,
    IFileStorageService fileService,
    IMapper mapper,
    ICacheService cacheService,
    IHttpContextAccessor accessor) : IProductService
{
    public async Task<ServiceResult> CreateProductAsync(CreateProductDto model)
    {
        try
        {
            var productEntity = mapper.Map<Product>(model);
            
            var result = await productRepository.CreateProductAsync(productEntity);
            
            if (result == 0)
                return ServiceResult.Fail("Failed to create product");

            await cacheService.RemoveAsync(CacheKeys.Products);
            
            return ServiceResult.Ok($"Product '{productEntity.Name}' created successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService.CreateProductAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> UpdateProductAsync(UpdateProductDto model)
    {
        try
        {
            var product = await productRepository.GetProductByIdAsync(model.Id);
            if (product == null) 
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);

            mapper.Map(model, product);
            
            product.UpdatedAt = DateTime.UtcNow;
            
            var result = await productRepository.UpdateProductAsync(product);

            if (result == 0)
                return ServiceResult.Fail("Failed to update product");
            
            await cacheService.RemoveAsync(CacheKeys.Products);
            
            return ServiceResult.Ok("Product updated successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService.UpdateProductAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> DeleteProductAsync(Guid productId)
    {
        try
        {
            var product = await productRepository.GetProductByIdAsync(productId);
            if (product == null) 
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            
            var result = await productRepository.DeleteProductAsync(product);
            
            if (result == 0) 
                return  ServiceResult.Fail("Failed to delete product");
            
            await cacheService.RemoveAsync(CacheKeys.Products);
            
            return ServiceResult.Ok("Product deleted successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService.DeleteProductAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<List<GetProductDto>>> GetProductsAsync(ProductFilter filter)
    {
        try
        {
            var userId = accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
            Log.Information("User {userId} tries to get {count} products", userId, filter.PageSize);

            var productsInCache = await cacheService.GetAsync<List<Product>>(CacheKeys.Products);

            if (productsInCache is null)
            {
                productsInCache = await productRepository.GetProductsAsync();

                await cacheService.AddAsync(CacheKeys.Products, productsInCache, DateTimeOffset.Now.AddMinutes(5));
            }

            var query = productsInCache.AsQueryable();
            
            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (filter.OnlyFeatured.HasValue)
                query = query.Where(p => p.IsFeatured == filter.OnlyFeatured.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice);
            
            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(p => p.Name.Contains(filter.Search, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "rating_asc":
                        query = query.OrderBy(p => p.Rating);
                        break;
                    case "rating_desc":
                        query = query.OrderByDescending(p => p.Rating);
                        break;
                    case "name":
                        query = query.OrderBy(p => p.Name);
                        break;
                    case "price_asc":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    default:
                        break;
                }
            }

            var totalCount = query.Count();
            var skip = (filter.PageNumber - 1) * filter.PageSize;
            var items = query
                .Skip(skip)
                .Take(filter.PageSize)
                .ToList();
            
            if (!items.Any())
                return new PaginationResponse<List<GetProductDto>>(HttpStatusCode.NotFound, "No products found");
            
            var mappedList = mapper.Map<List<GetProductDto>>(items);

            Log.Information("Found {count} elements", totalCount);
            return new PaginationResponse<List<GetProductDto>>(mappedList, totalCount, filter.PageNumber, filter.PageSize);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService.GetProductsAsync");
            return ServiceResult<List<GetProductDto>>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<GetProductDto>> GetProductByIdAsync(Guid productId)
    {
        try
        {
            var product = await productRepository.GetProductByIdAsync(productId);
            
            if (product == null)
                return ServiceResult<GetProductDto>.Fail("Product not found", HttpStatusCode.NotFound);
            
            var mappedProduct = mapper.Map<GetProductDto>(product);
            
            return ServiceResult<GetProductDto>.Ok(mappedProduct);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService");
            return ServiceResult<GetProductDto>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> AddImageToProductAsync(AddProductImageDto productImageDto)
    {
        try
        {
            var product = await productRepository.GetProductByIdAsync(productImageDto.ProductId);
            if (product == null)
                return ServiceResult.Fail("Not found the product", HttpStatusCode.NotFound);

            if (productImageDto.Image.Length > 5 * 1024 * 1024)
                return ServiceResult.Fail("File size must be less than 5 MB");

            if (productImageDto.IsMain)
            {
                if (product.Images.Any(img => img.IsMain))
                    return ServiceResult.Fail("Only one image can be the main");
            }
        
            var mappedImage = mapper.Map<ProductImage>(productImageDto);
            
            var ext = Path.GetExtension(productImageDto.Image.FileName);
            
            if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                mappedImage.ImageUrl = await fileService.SaveFileAsync(productImageDto.Image, "images/products/");
            }
            else
            {
                return ServiceResult.Fail("Not supported image type");
            }
        
            mappedImage.SortOrder = await productRepository.GetNextSortOrderAsync(product.Id);
        
            await productRepository.AddImageToProductAsync(mappedImage);
        
            return ServiceResult.Ok("Added an image to product");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService.AddImageToProductAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> RemoveImageFromProductAsync(Guid productId, Guid imageId)
    {
        try
        {
            var product = await productRepository.GetProductByIdAsync(productId);
            if (product == null)
                return ServiceResult.Fail("Not found the product", HttpStatusCode.NotFound);
            
            var productImage = product.Images.FirstOrDefault(img => img.Id == imageId);
            if (productImage == null)
                return ServiceResult.Fail("Not found the image", HttpStatusCode.NotFound);
            
            await fileService.DeleteFileAsync(productImage.ImageUrl);
            await productRepository.DeleteImageFromProductAsync(productImage);

            return ServiceResult.Ok("Deleted product-image successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService.RemoveImageFromProductsAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }
}