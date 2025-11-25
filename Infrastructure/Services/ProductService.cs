using System.Net;
using Application.DTOs.Product;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Responses;
using Serilog;

namespace Infrastructure.Services;

public class ProductService(
    IProductRepository productRepository,
    IMapper mapper) : IProductService
{
    public async Task<ServiceResult> CreateProductAsync(CreateProductDto model)
    {
        try
        {
            var productEntity = mapper.Map<Product>(model);
            
            var result = await productRepository.CreateProductAsync(productEntity);
            
            if (result == 0)
                return ServiceResult.Fail("Failed to create product");
            
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
            
            return ServiceResult.Ok("Product deleted successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in ProductService.DeleteProductAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<List<GetProductDto>>> GetProductsAsync()
    {
        try
        {
            var products = await productRepository.GetProductsAsync();
            if (products.Count == 0) 
                return ServiceResult<List<GetProductDto>>.Fail("No products found", HttpStatusCode.NotFound);

            var mappedList = mapper.Map<List<GetProductDto>>(products);
            
            return ServiceResult<List<GetProductDto>>.Ok(mappedList);
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
}