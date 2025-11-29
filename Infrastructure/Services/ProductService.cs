using System.Net;
using System.Security.Claims;
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
    ICategoryRepository categoryRepository,
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

            if (model.CategoryId.HasValue)
            {
                var existingCategory = await categoryRepository.GetCategoryByIdAsync(model.CategoryId.Value);
                if (existingCategory == null)
                    return ServiceResult.Fail("Not found the category");
            }

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
            Log.Information("Trying to delete product {pId}", productId);
            var product = await productRepository.GetProductByIdAsync(productId);
            if (product == null)
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);

            var result = await productRepository.DeleteProductAsync(product);

            if (result == 0)
                return ServiceResult.Fail("Failed to delete product");

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
            var userId = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
            Log.Information("User {UserId} requests {PageSize} products", userId, filter.PageSize);

            var cachedProducts = await cacheService.GetAsync<List<GetProductDto>>(CacheKeys.Products);

            if (cachedProducts is null)
            {
                var products = await productRepository.GetProductsAsync();
                cachedProducts = mapper.Map<List<GetProductDto>>(products);

                await cacheService.AddAsync(CacheKeys.Products, cachedProducts, DateTimeOffset.Now.AddMinutes(5));
                Log.Information("Products loaded from DB and cached");
            }
            else
            {
                Log.Information("Products loaded from cache");
            }

            var query = cachedProducts.AsQueryable();

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (filter.OnlyFeatured.HasValue)
                query = query.Where(p => p.IsFeatured == filter.OnlyFeatured.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice);

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(p => p.Name.Contains(filter.Search, StringComparison.OrdinalIgnoreCase));

            query = filter.SortBy?.ToLower() switch
            {
                "rating_asc" => query.OrderBy(p => p.Rating),
                "rating_desc" => query.OrderByDescending(p => p.Rating),
                "name" => query.OrderBy(p => p.Name),
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                _ => query
            };

            var totalCount = query.Count();

            var items = query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            if (!items.Any())
                return new PaginationResponse<List<GetProductDto>>(HttpStatusCode.NotFound, "No products found");

            Log.Information("Filtered result: {Count} products", totalCount);

            return new PaginationResponse<List<GetProductDto>>(items, totalCount, filter.PageNumber, filter.PageSize);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService.GetProductsAsync");
            return ServiceResult<List<GetProductDto>>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<GetProductDto>> GetProductBySlugAsync(string slug)
    {
        try
        {
            var product = await productRepository.GetProductBySlugAsync(slug);
            if (product == null)
                return ServiceResult<GetProductDto>.Fail("Not found product", HttpStatusCode.NotFound);

            var mappedProduct = mapper.Map<GetProductDto>(product);
            
            return ServiceResult<GetProductDto>.Ok(mappedProduct);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService.GetProductBySlugAsync");
            return ServiceResult<GetProductDto>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<GetProductDto>> GetProductByIdAsync(string slug)
    {
        try
        {
            var product = await productRepository.GetProductBySlugAsync(slug);

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

    public async Task<ServiceResult> AddImageToProductAsync(Guid productId, AddProductImageDto productImageDto)
    {
        try
        {
            var product = await productRepository.GetProductByIdAsync(productId);
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
            mappedImage.ProductId = productId;

            var ext = Path.GetExtension(productImageDto.Image.FileName);

            if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                mappedImage.ImageUrl = await fileService.SaveFileAsync(productImageDto.Image, "images/products/");
                await cacheService.RemoveAsync(CacheKeys.Products);
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

    public async Task<ServiceResult> AddReviewToProductAsync(Guid productId, int stars)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var guidUserId = Guid.Parse(userId!);
            Log.Information("User {UserId} tries to add a review to product {productId}", userId, productId);   
            
            var existingProduct = await productRepository.GetProductByIdAsync(productId);
            if (existingProduct == null)
                return ServiceResult.Fail("Not found the product", HttpStatusCode.NotFound);
            
            if (existingProduct.Reviews.Any(r => r.UserId == guidUserId))
                return ServiceResult.Fail("Review already exists");

            if (stars <= 0 || stars > 5)
                return ServiceResult.Fail("Stars must be between 1 and 5.");

            var mappedReview = new Review
            {
                ProductId = productId,
                UserId = guidUserId,
                Stars = stars
            };
            
            existingProduct.Reviews.Add(mappedReview);
            existingProduct.Rating = (int)Math.Round(existingProduct.Reviews.Average(p => p.Stars));
            
            var result = await productRepository.AddReviewToProductAsync(mappedReview);

            if (result == 0)
            {
                Log.Warning("Failed to add a review to product");
                return ServiceResult.Fail("Failed to add a review");
            }

            Log.Information("User {uId} added a review to product {pId}", userId, productId);
            return ServiceResult.Ok("Added a review successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService.AddReviewToProductAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> UpdateReviewOfProductAsync(Guid productId, int stars)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var guidUserId = Guid.Parse(userId!);
            Log.Information("User {UserId} tries to update a review of product {productId}", userId, productId);   
            
            if (stars < 1 || stars > 5)
                return ServiceResult.Fail("Stars must be between 1 and 5.");
            
            var existingProduct = await productRepository.GetProductByIdAsync(productId);
            if (existingProduct == null)
                return ServiceResult.Fail("Not found the product", HttpStatusCode.NotFound);

            var review = existingProduct.Reviews.FirstOrDefault(r => r.UserId == guidUserId);
            if (review == null)
                return ServiceResult.Fail("Not found the review", HttpStatusCode.NotFound);

            if (review.UserId != guidUserId)
                return ServiceResult.Fail("Forbidden");
            
            review.Stars = stars;
            
            existingProduct.Rating = (int) Math.Round(existingProduct.Reviews.Average(p => p.Stars));
            
            var result = await productRepository.UpdateReviewOfProductAsync(review);
            
            if (result == 0)
            {
                Log.Warning("Failed to update the review");
                return ServiceResult.Fail("Failed to update the review");
            }

            Log.Information("User {uId} updated the review {reviewId}", userId, review.Id);
            return ServiceResult.Ok("Updated review successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService.UpdateReviewOfProductAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> DeleteReviewFromProductAsync(Guid productId, Guid reviewId)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var guidUserId = Guid.Parse(userId!);
            Log.Information("User {UserId} tries to delete a review of product {productId}", userId, productId);   
            
            var existingProduct = await productRepository.GetProductByIdAsync(productId);
            if (existingProduct == null)
                return ServiceResult.Fail("Not found the product", HttpStatusCode.NotFound);

            var review = existingProduct.Reviews
                .FirstOrDefault(r => r.Id == reviewId && r.UserId == guidUserId);

            if (review == null)
                return ServiceResult.Fail("Not found the review", HttpStatusCode.NotFound);
            
            if (review.UserId != guidUserId)
                return ServiceResult.Fail("Forbidden");

            existingProduct.Reviews.Remove(review);
            existingProduct.Rating = existingProduct.Reviews.Count > 0 
                ? (int)Math.Round(existingProduct.Reviews.Average(p => p.Stars)) 
                : 0;

            var result = await productRepository.DeleteReviewFromProductAsync(review);
            
            if (result == 0)
            {
                Log.Warning("Failed to delete the review");
                return ServiceResult.Fail("Failed to delete the review");
            }

            Log.Information("User {uId} deleted his review from product {pId}", userId, existingProduct.Id);
            return ServiceResult.Ok("Deleted review successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService.DeleteReviewFromProductAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }
}